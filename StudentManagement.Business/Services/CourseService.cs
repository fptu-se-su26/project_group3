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

<<<<<<< HEAD
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
=======
        public async Task<IEnumerable<Subject>> GetAllSubjectsAsync(string? searchKeyword = null)
        {
            var query = _context.Subjects.AsNoTracking().AsQueryable();
            if (!string.IsNullOrWhiteSpace(searchKeyword))
                query = query.Where(s => s.SubjectId.Contains(searchKeyword) || s.SubjectName.Contains(searchKeyword));
            return await query.ToListAsync();
        }

        public async Task<(bool IsSuccess, string Message)> AddSubjectAsync(Subject subject)
        {
            if (!ValidationHelper.IsRequired(subject.SubjectId) || !ValidationHelper.IsRequired(subject.SubjectName))
                return (false, "Subject ID and Name are required.");
            if (!ValidationHelper.IsPositive(subject.Credits))
                return (false, "Credits must be greater than 0.");
            if (await _context.Subjects.AnyAsync(s => s.SubjectId == subject.SubjectId))
                return (false, "Subject ID already exists.");

            await _context.Subjects.AddAsync(subject);
            await _context.SaveChangesAsync();
            return (true, "Subject added successfully.");
        }

        public async Task<(bool IsSuccess, string Message)> UpdateSubjectAsync(Subject subject)
        {
            if (!ValidationHelper.IsPositive(subject.Credits))
                return (false, "Credits must be greater than 0.");

            _context.Subjects.Update(subject);
            await _context.SaveChangesAsync();
            return (true, "Subject updated successfully.");
        }

        public async Task<(bool IsSuccess, string Message)> DeactivateSubjectAsync(string subjectId)
        {
            var subject = await _context.Subjects.FindAsync(subjectId);
            if (subject == null) return (false, "Subject not found.");

            var hasOpenSections = await _context.CourseSections.AnyAsync(cs => cs.SubjectId == subjectId && cs.Status == CourseSectionStatus.Opened);
            if (hasOpenSections) return (false, "Cannot deactivate: subject has open course sections.");

            subject.Status = SubjectStatus.Inactive;
            await _context.SaveChangesAsync();
            return (true, "Subject deactivated.");
>>>>>>> origin/Main
        }

        public async Task<IEnumerable<CourseSection>> GetAllCourseSectionsAsync()
        {
            return await _context.CourseSections
                .Include(cs => cs.Subject)
                .Include(cs => cs.Lecturer)
                .Include(cs => cs.Semester)
                .ToListAsync();
        }

<<<<<<< HEAD
        public async Task AddCourseSectionAsync(CourseSection section)
        {
            await _context.CourseSections.AddAsync(section);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCourseSectionAsync(CourseSection section)
        {
            _context.CourseSections.Update(section);
            await _context.SaveChangesAsync();
=======
        public async Task<(bool IsSuccess, string Message)> AddCourseSectionAsync(CourseSection section)
        {
            if (!ValidationHelper.IsRequired(section.SectionId) || !ValidationHelper.IsRequired(section.SubjectId))
                return (false, "Section ID and Subject are required.");
            if (!ValidationHelper.IsPositive(section.Capacity))
                return (false, "Capacity must be greater than 0.");
            if (await _context.CourseSections.AnyAsync(cs => cs.SectionId == section.SectionId))
                return (false, "Section ID already exists.");

            if (!string.IsNullOrWhiteSpace(section.LecturerId))
            {
                var conflict = await HasLecturerConflictAsync(section.LecturerId, section.SemesterId, section.DayOfWeek, section.StartTime, section.EndTime, null);
                if (conflict) return (false, "Lecturer has a schedule conflict with another section.");
            }

            await _context.CourseSections.AddAsync(section);
            await _context.SaveChangesAsync();
            return (true, "Course section created successfully.");
        }

        public async Task<(bool IsSuccess, string Message)> UpdateCourseSectionAsync(CourseSection section)
        {
            if (!ValidationHelper.IsPositive(section.Capacity))
                return (false, "Capacity must be greater than 0.");

            _context.CourseSections.Update(section);
            await _context.SaveChangesAsync();
            return (true, "Course section updated successfully.");
        }

        private async Task<bool> HasLecturerConflictAsync(string lecturerId, string semesterId, string dayOfWeek, TimeSpan startTime, TimeSpan endTime, string? excludeSectionId)
        {
            var otherSections = await _context.CourseSections
                .Where(cs => cs.LecturerId == lecturerId
                          && cs.SemesterId == semesterId
                          && cs.DayOfWeek == dayOfWeek
                          && cs.Status != CourseSectionStatus.Cancelled
                          && cs.SectionId != excludeSectionId)
                .ToListAsync();

            return otherSections.Any(cs =>
                (startTime >= cs.StartTime && startTime < cs.EndTime) ||
                (endTime > cs.StartTime && endTime <= cs.EndTime) ||
                (startTime <= cs.StartTime && endTime >= cs.EndTime));
        }

        public async Task<(bool IsSuccess, string Message)> AssignLecturerToSectionAsync(string sectionId, string lecturerId)
        {
            var section = await _context.CourseSections.FindAsync(sectionId);
            if (section == null) return (false, "Course section not found.");

            var lecturer = await _context.Lecturers.FindAsync(lecturerId);
            if (lecturer == null || lecturer.Status != LecturerStatus.Active)
                return (false, "Lecturer must be active.");

            var conflict = await HasLecturerConflictAsync(lecturerId, section.SemesterId, section.DayOfWeek, section.StartTime, section.EndTime, sectionId);
            if (conflict) return (false, "Lecturer has a schedule conflict with another section.");

            section.LecturerId = lecturerId;
            await _context.SaveChangesAsync();
            return (true, "Lecturer assigned to section.");
>>>>>>> origin/Main
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
<<<<<<< HEAD
            
            // Check Capacity
            var currentEnrolled = section.Registrations.Count(r => r.Status != RegistrationStatus.Cancelled);
            if (currentEnrolled >= section.Capacity) return (false, "Course section is full.");

            // Check if already registered
            var alreadyRegistered = section.Registrations.Any(r => r.StudentId == studentId && r.Status != RegistrationStatus.Cancelled);
            if (alreadyRegistered) return (false, "You are already registered for this section.");

            // Check Schedule Conflict
=======

            var currentEnrolled = section.Registrations.Count(r => r.Status != RegistrationStatus.Cancelled);
            if (currentEnrolled >= section.Capacity) return (false, "Course section is full.");

            var alreadyRegistered = section.Registrations.Any(r => r.StudentId == studentId && r.Status != RegistrationStatus.Cancelled);
            if (alreadyRegistered) return (false, "You are already registered for this section.");

>>>>>>> origin/Main
            var studentTimetable = await GetStudentTimetableAsync(studentId, section.SemesterId);
            foreach (var enrolled in studentTimetable)
            {
                if (enrolled.DayOfWeek == section.DayOfWeek)
                {
                    if ((section.StartTime >= enrolled.StartTime && section.StartTime < enrolled.EndTime) ||
                        (section.EndTime > enrolled.StartTime && section.EndTime <= enrolled.EndTime) ||
                        (section.StartTime <= enrolled.StartTime && section.EndTime >= enrolled.EndTime))
                    {
                        return (false, $"Schedule conflict with {enrolled.SubjectId} on {enrolled.DayOfWeek}.");
                    }
                }
            }

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

<<<<<<< HEAD
        public async Task<IEnumerable<Student>> GetStudentsBySectionAsync(string sectionId)
        {
            return await _context.Registrations
=======
        public async Task<(bool IsSuccess, string Message)> CancelRegistrationAsync(int registrationId)
        {
            var registration = await _context.Registrations
                .Include(r => r.Grade)
                .FirstOrDefaultAsync(r => r.RegistrationId == registrationId);

            if (registration == null) return (false, "Registration not found.");
            if (registration.Status == RegistrationStatus.Cancelled) return (false, "Registration already cancelled.");

            var hasGrade = registration.Grade != null && (registration.Grade.Assignment.HasValue || registration.Grade.ProgressTest.HasValue
                || registration.Grade.Practical.HasValue || registration.Grade.FinalExam.HasValue);
            if (hasGrade) return (false, "Cannot cancel registration: grades have already been entered.");

            registration.Status = RegistrationStatus.Cancelled;
            await _context.SaveChangesAsync();
            return (true, "Registration cancelled.");
        }

        public async Task<(bool IsSuccess, string Message)> CancelRegistrationAsync(string studentId, string sectionId)
        {
            var registration = await _context.Registrations
                .FirstOrDefaultAsync(r => r.StudentId == studentId && r.SectionId == sectionId && r.Status != RegistrationStatus.Cancelled);

            if (registration == null) return (false, "Registration not found.");
            return await CancelRegistrationAsync(registration.RegistrationId);
        }

        public async Task<IEnumerable<Student>> GetStudentsBySectionAsync(string sectionId)
        {
            return await _context.Registrations
                .AsNoTracking()
>>>>>>> origin/Main
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
