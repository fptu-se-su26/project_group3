using System.Threading.Tasks;

namespace StudentManagement.Business.Interfaces
{
    public interface IAuditService
    {
        Task LogAsync(int? userId, string action);
    }
}
