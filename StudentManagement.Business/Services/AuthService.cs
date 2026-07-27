using Microsoft.EntityFrameworkCore;
using StudentManagement.Business.Interfaces;
using StudentManagement.Business.Security;
using StudentManagement.DataAccess;
using StudentManagement.Domain.Entities;
using System.Threading.Tasks;

namespace StudentManagement.Business.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly IAuditService _auditService;

        public AuthService(AppDbContext context, IAuditService auditService)
        {
            _context = context;
            _auditService = auditService;
        }

        public async Task<(bool IsSuccess, string Message, User? User)> AuthenticateAsync(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                return (false, "Username and password are required.", null);

            var user = await _context.Users
                .AsNoTracking()
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Username == username);

            if (user == null)
                return (false, "Invalid username or password.", null);

            if (!user.Status)
                return (false, "Your account has been locked. Please contact Administrator.", null);

            if (!PasswordHasher.Verify(password, user.PasswordHash))
            {
                await _auditService.LogAsync(user.UserId, "Failed login attempt.");
                return (false, "Invalid username or password.", null);
            }

            await _auditService.LogAsync(user.UserId, "Logged in.");
            return (true, "Login successful.", user);
        }
    }
}
