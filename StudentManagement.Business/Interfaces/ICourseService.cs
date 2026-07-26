using StudentManagement.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StudentManagement.Business.Interfaces
{
    public interface ICourseService
    {
        // Subjects
        Task<IEnumerable<Subject>> GetAllSubjectsAsync();
        Task AddSubjectAsync(Subject subject);
        Task UpdateSubjectAsync(Subject subject);

        // Course Sections
        Task<IEnumerable<CourseSection>> GetAllCourseSectionsAsync();
        Task AddCourseSectionAsync(CourseSection section);
        Task UpdateCourseSectionAsync(CourseSection section);

        // Registration & Logic
        Task<IEnumerable<CourseSection>> GetAvailableSectionsForSemesterAsync(string semesterId);
        Task<IEnumerable<CourseSection>> GetStudentTimetableAsync(string studentId, string semesterId);
        Task<(bool IsSuccess, string Message)> RegisterCourseAsync(string studentId, string sectionId);
        Task<IEnumerable<Student>> GetStudentsBySectionAsync(string sectionId);
        Task<int> CalculateTotalCreditsAsync(string studentId, string semesterId);
    }
}
