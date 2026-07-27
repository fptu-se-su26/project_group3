using StudentManagement.Domain.Entities;
using System.Threading.Tasks;

namespace StudentManagement.Business.Interfaces
{
    public interface IAuthService
    {
        Task<(bool IsSuccess, string Message, User? User)> AuthenticateAsync(string username, string password);
    }
}
