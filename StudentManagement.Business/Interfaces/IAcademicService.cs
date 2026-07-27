using StudentManagement.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StudentManagement.Business.Interfaces
{
    public interface IAcademicService
    {
        // Majors
        Task<IEnumerable<Major>> GetAllMajorsAsync(string? searchKeyword = null);
        Task<(bool IsSuccess, string Message)> AddMajorAsync(Major major);
        Task<(bool IsSuccess, string Message)> UpdateMajorAsync(Major major);
        Task<(bool IsSuccess, string Message)> DeactivateMajorAsync(string majorId);

        // Classes
        Task<IEnumerable<Class>> GetAllClassesAsync();
        Task<(bool IsSuccess, string Message)> AddClassAsync(Class cls);
        Task<(bool IsSuccess, string Message)> UpdateClassAsync(Class cls);

        // Lecturers
        Task<IEnumerable<Lecturer>> GetAllLecturersAsync(string? searchKeyword = null);
        Task<(bool IsSuccess, string Message)> AddLecturerAsync(Lecturer lecturer);
        Task<(bool IsSuccess, string Message)> UpdateLecturerAsync(Lecturer lecturer);
        Task<(bool IsSuccess, string Message)> DeactivateLecturerAsync(string lecturerId);

        // Class-Student & Class-Lecturer operations
        Task<IEnumerable<Student>> GetStudentsByClassAsync(string classId);
        Task<(bool IsSuccess, string Message)> AssignHomeroomLecturerAsync(string classId, string lecturerId);
        Task<(bool IsSuccess, string Message)> AssignStudentToClassAsync(string studentId, string newClassId);

        // Dashboard
        Task<(int totalStudents, int totalLecturers, int totalClasses, int totalMajors, int activeStudents, int graduatedStudents)> GetDashboardSummaryAsync();
    }
}
