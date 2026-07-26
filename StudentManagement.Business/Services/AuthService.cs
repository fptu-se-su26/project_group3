using Microsoft.EntityFrameworkCore;
using StudentManagement.Business.Interfaces;
using StudentManagement.DataAccess;
using StudentManagement.Domain.Entities;
using System.Threading.Tasks;

namespace StudentManagement.Business.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;

        public AuthService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<(bool IsSuccess, string Message, User? User)> AuthenticateAsync(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                return (false, "Username and password are required.", null);

            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Username == username);

            if (user == null)
                return (false, "Invalid username or password.", null);

            if (!user.Status)
                return (false, "Your account has been locked. Please contact Administrator.", null);

            // In a real application, verify hash here. For this academic project, we assume plain string equality or a simple hash matching.
            if (user.PasswordHash != password)
                return (false, "Invalid username or password.", null);

            return (true, "Login successful.", user);
        }
    }
}
