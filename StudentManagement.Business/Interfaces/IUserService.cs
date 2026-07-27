using StudentManagement.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StudentManagement.Business.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAllUsersAsync(string? searchKeyword = null);
        Task<IEnumerable<Role>> GetAllRolesAsync();
        Task<(bool IsSuccess, string Message)> CreateUserAsync(string username, string password, int roleId);
        Task<(bool IsSuccess, string Message)> AssignRoleAsync(int userId, int roleId);
        Task<(bool IsSuccess, string Message)> SetLockStatusAsync(int userId, bool isActive);
        Task<(bool IsSuccess, string Message)> ChangePasswordAsync(int userId, string currentPassword, string newPassword, string confirmPassword);
    }
}
