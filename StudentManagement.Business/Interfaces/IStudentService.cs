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
        Task AddStudentAsync(Student student);
        Task UpdateStudentAsync(Student student);
        Task UpdateStudentStatusAsync(string studentId, StudentStatus newStatus);
        Task ExportToJsonAsync(string filePath, IEnumerable<Student> students);
        Task ExportToXmlAsync(string filePath, IEnumerable<Student> students);
        Task<(int success, int fail)> ImportFromJsonAsync(string filePath);
    }
}
