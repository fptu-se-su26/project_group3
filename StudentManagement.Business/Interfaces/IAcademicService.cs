using StudentManagement.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StudentManagement.Business.Interfaces
{
    public interface IAcademicService
    {
        // Majors
        Task<IEnumerable<Major>> GetAllMajorsAsync();
        Task AddMajorAsync(Major major);
        Task UpdateMajorAsync(Major major);
        
        // Classes
        Task<IEnumerable<Class>> GetAllClassesAsync();
        Task AddClassAsync(Class cls);
        Task UpdateClassAsync(Class cls);
        
        // Lecturers
        Task<IEnumerable<Lecturer>> GetAllLecturersAsync();
        Task AddLecturerAsync(Lecturer lecturer);
        Task UpdateLecturerAsync(Lecturer lecturer);
        
        // Class-Student & Class-Lecturer operations
        Task<IEnumerable<Student>> GetStudentsByClassAsync(string classId);
        Task AssignHomeroomLecturerAsync(string classId, string lecturerId);
        Task AssignStudentToClassAsync(string studentId, string newClassId);
    }
}
