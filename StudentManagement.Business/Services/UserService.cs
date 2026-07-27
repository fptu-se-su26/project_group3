using Microsoft.EntityFrameworkCore;
using StudentManagement.Business.Interfaces;
using StudentManagement.Business.Security;
using StudentManagement.Business.Validators;
using StudentManagement.DataAccess;
using StudentManagement.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentManagement.Business.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;
        private readonly IAuditService _auditService;

        public UserService(AppDbContext context, IAuditService auditService)
        {
            _context = context;
            _auditService = auditService;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync(string? searchKeyword = null)
        {
            var query = _context.Users.AsNoTracking().Include(u => u.Role).AsQueryable();
            if (!string.IsNullOrWhiteSpace(searchKeyword))
                query = query.Where(u => u.Username.Contains(searchKeyword));

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<Role>> GetAllRolesAsync()
        {
            return await _context.Roles.AsNoTracking().ToListAsync();
        }

        public async Task<(bool IsSuccess, string Message)> CreateUserAsync(string username, string password, int roleId)
        {
            if (!ValidationHelper.IsRequired(username))
                return (false, "Username is required.");
            if (!ValidationHelper.IsValidPassword(password))
                return (false, $"Password must be at least {ValidationHelper.MinPasswordLength} characters.");

            var exists = await _context.Users.AnyAsync(u => u.Username == username);
            if (exists)
                return (false, "Username already exists.");

            var role = await _context.Roles.FindAsync(roleId);
            if (role == null)
                return (false, "Selected role does not exist.");

            var user = new User
            {
                Username = username,
                PasswordHash = PasswordHasher.Hash(password),
                RoleId = roleId,
                Status = true
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            await _auditService.LogAsync(user.UserId, $"Created user account '{username}'.");

            return (true, "Account created successfully.");
        }

        public async Task<(bool IsSuccess, string Message)> AssignRoleAsync(int userId, int roleId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return (false, "User not found.");

            var role = await _context.Roles.FindAsync(roleId);
            if (role == null) return (false, "Selected role does not exist.");

            user.RoleId = roleId;
            await _context.SaveChangesAsync();
            await _auditService.LogAsync(userId, $"Assigned role '{role.RoleName}'.");

            return (true, "Role assigned successfully.");
        }

        public async Task<(bool IsSuccess, string Message)> SetLockStatusAsync(int userId, bool isActive)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return (false, "User not found.");

            user.Status = isActive;
            await _context.SaveChangesAsync();
            await _auditService.LogAsync(userId, isActive ? "Unlocked account." : "Locked account.");

            return (true, isActive ? "Account unlocked." : "Account locked.");
        }

        public async Task<(bool IsSuccess, string Message)> ChangePasswordAsync(int userId, string currentPassword, string newPassword, string confirmPassword)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return (false, "User not found.");

            if (!PasswordHasher.Verify(currentPassword, user.PasswordHash))
                return (false, "Current password is incorrect.");

            if (!ValidationHelper.IsValidPassword(newPassword))
                return (false, $"New password must be at least {ValidationHelper.MinPasswordLength} characters.");

            if (newPassword != confirmPassword)
                return (false, "New password and confirmation do not match.");

            user.PasswordHash = PasswordHasher.Hash(newPassword);
            await _context.SaveChangesAsync();
            await _auditService.LogAsync(userId, "Changed password.");

            return (true, "Password changed successfully.");
        }
    }
}
