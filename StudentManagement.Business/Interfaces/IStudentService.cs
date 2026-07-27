using StudentManagement.Domain.Entities;
using StudentManagement.Domain.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StudentManagement.Business.Interfaces
{
    public interface IStudentService
    {
        Task<IEnumerable<Student>> GetAllStudentsAsync(string? searchKeyword = null, string? classId = null, string? majorId = null, StudentStatus? status = null);
        Task<Student?> GetStudentByIdAsync(string id);
        Task<(bool IsSuccess, string Message)> AddStudentAsync(Student student);
        Task<(bool IsSuccess, string Message)> UpdateStudentAsync(Student student);
        Task<(bool IsSuccess, string Message)> UpdateStudentStatusAsync(string studentId, StudentStatus newStatus, string? reason);
        Task ExportToJsonAsync(string filePath, IEnumerable<Student> students);
        Task ExportToXmlAsync(string filePath, IEnumerable<Student> students);
        Task<(int success, int fail, List<string> errors)> ImportFromJsonAsync(string filePath);
    }
}
