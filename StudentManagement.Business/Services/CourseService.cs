using Microsoft.EntityFrameworkCore;
using StudentManagement.Business.Interfaces;
using StudentManagement.DataAccess;
using StudentManagement.Domain.Entities;
using StudentManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentManagement.Business.Services
{
    public class CourseService : ICourseService
    {
        private readonly AppDbContext _context;

        public CourseService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Subject>> GetAllSubjectsAsync()
        {
            return await _context.Subjects.ToListAsync();
        }

        public async Task AddSubjectAsync(Subject subject)
        {
            await _context.Subjects.AddAsync(subject);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateSubjectAsync(Subject subject)
        {
            _context.Subjects.Update(subject);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<CourseSection>> GetAllCourseSectionsAsync()
        {
            return await _context.CourseSections
                .Include(cs => cs.Subject)
                .Include(cs => cs.Lecturer)
                .Include(cs => cs.Semester)
                .ToListAsync();
        }

        public async Task AddCourseSectionAsync(CourseSection section)
        {
            await _context.CourseSections.AddAsync(section);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCourseSectionAsync(CourseSection section)
        {
            _context.CourseSections.Update(section);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<CourseSection>> GetAvailableSectionsForSemesterAsync(string semesterId)
        {
            return await _context.CourseSections
                .Include(cs => cs.Subject)
                .Include(cs => cs.Lecturer)
                .Where(cs => cs.SemesterId == semesterId && cs.Status == CourseSectionStatus.Opened)
                .ToListAsync();
        }

        public async Task<IEnumerable<CourseSection>> GetStudentTimetableAsync(string studentId, string semesterId)
        {
            return await _context.Registrations
                .Where(r => r.StudentId == studentId && r.CourseSection.SemesterId == semesterId && r.Status != RegistrationStatus.Cancelled)
                .Include(r => r.CourseSection)
                .ThenInclude(cs => cs.Subject)
                .Select(r => r.CourseSection)
                .ToListAsync();
        }

        public async Task<(bool IsSuccess, string Message)> RegisterCourseAsync(string studentId, string sectionId)
        {
            var section = await _context.CourseSections
                .Include(cs => cs.Registrations)
                .FirstOrDefaultAsync(cs => cs.SectionId == sectionId);

            if (section == null) return (false, "Course section not found.");
            if (section.Status != CourseSectionStatus.Opened) return (false, "Course section is not open for registration.");
            
            // Check Capacity
            var currentEnrolled = section.Registrations.Count(r => r.Status != RegistrationStatus.Cancelled);
            if (currentEnrolled >= section.Capacity) return (false, "Course section is full.");

            // Check if already registered
            var alreadyRegistered = section.Registrations.Any(r => r.StudentId == studentId && r.Status != RegistrationStatus.Cancelled);
            if (alreadyRegistered) return (false, "You are already registered for this section.");

            // Check Schedule Conflict
            var studentTimetable = await GetStudentTimetableAsync(studentId, section.SemesterId);
            foreach (var enrolled in studentTimetable)
            {
                if (enrolled.DayOfWeek == section.DayOfWeek)
                {
                    // Time overlap logic
                    if ((section.StartTime >= enrolled.StartTime && section.StartTime < enrolled.EndTime) ||
                        (section.EndTime > enrolled.StartTime && section.EndTime <= enrolled.EndTime) ||
                        (section.StartTime <= enrolled.StartTime && section.EndTime >= enrolled.EndTime))
                    {
                        return (false, $"Schedule conflict with {enrolled.SubjectId} on {enrolled.DayOfWeek}.");
                    }
                }
            }

            // Register
            var registration = new Registration
            {
                StudentId = studentId,
                SectionId = sectionId,
                RegistrationDate = DateTime.Now,
                Status = RegistrationStatus.Registered
            };

            await _context.Registrations.AddAsync(registration);
            await _context.SaveChangesAsync();

            return (true, "Registration successful!");
        }

        public async Task<IEnumerable<Student>> GetStudentsBySectionAsync(string sectionId)
        {
            return await _context.Registrations
                .Where(r => r.SectionId == sectionId && r.Status != RegistrationStatus.Cancelled)
                .Include(r => r.Student)
                .Select(r => r.Student)
                .ToListAsync();
        }

        public async Task<int> CalculateTotalCreditsAsync(string studentId, string semesterId)
        {
            var timetable = await GetStudentTimetableAsync(studentId, semesterId);
            return timetable.Sum(cs => cs.Subject.Credits);
        }
    }
}
