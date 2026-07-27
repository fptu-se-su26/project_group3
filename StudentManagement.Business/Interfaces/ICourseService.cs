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
        Task<IEnumerable<Subject>> GetAllSubjectsAsync(string? searchKeyword = null);
        Task<(bool IsSuccess, string Message)> AddSubjectAsync(Subject subject);
        Task<(bool IsSuccess, string Message)> UpdateSubjectAsync(Subject subject);
        Task<(bool IsSuccess, string Message)> DeactivateSubjectAsync(string subjectId);

        // Course Sections
        Task<IEnumerable<CourseSection>> GetAllCourseSectionsAsync();
        Task<(bool IsSuccess, string Message)> AddCourseSectionAsync(CourseSection section);
        Task<(bool IsSuccess, string Message)> UpdateCourseSectionAsync(CourseSection section);
        Task<(bool IsSuccess, string Message)> AssignLecturerToSectionAsync(string sectionId, string lecturerId);

        // Registration & Logic
        Task<IEnumerable<CourseSection>> GetAvailableSectionsForSemesterAsync(string semesterId);
        Task<IEnumerable<CourseSection>> GetStudentTimetableAsync(string studentId, string semesterId);
        Task<(bool IsSuccess, string Message)> RegisterCourseAsync(string studentId, string sectionId);
        Task<IEnumerable<Student>> GetStudentsBySectionAsync(string sectionId);
        Task<int> CalculateTotalCreditsAsync(string studentId, string semesterId);
        Task<(bool IsSuccess, string Message)> CancelRegistrationAsync(int registrationId);
        Task<(bool IsSuccess, string Message)> CancelRegistrationAsync(string studentId, string sectionId);
        Task<IEnumerable<Student>> GetStudentsBySectionAsync(string sectionId);
        Task<int> CalculateTotalCreditsAsync(string studentId, string semesterId);
        Task<IEnumerable<Semester>> GetAllSemestersAsync();
    }
}
