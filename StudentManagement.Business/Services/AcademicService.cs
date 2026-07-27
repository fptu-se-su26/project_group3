using Microsoft.EntityFrameworkCore;
using StudentManagement.Business.Interfaces;
using StudentManagement.Business.Validators;
using StudentManagement.DataAccess;
using StudentManagement.Domain.Entities;
using StudentManagement.Domain.Enums;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentManagement.Business.Services
{
    public class AcademicService : IAcademicService
    {
        private readonly AppDbContext _context;

        public AcademicService(AppDbContext context)
        {
            _context = context;
        }

        // Majors
        public async Task<IEnumerable<Major>> GetAllMajorsAsync(string? searchKeyword = null)
        {
            var query = _context.Majors.AsNoTracking().AsQueryable();
            if (!string.IsNullOrWhiteSpace(searchKeyword))
                query = query.Where(m => m.MajorId.Contains(searchKeyword) || m.MajorName.Contains(searchKeyword));
            return await query.ToListAsync();
        }

        public async Task<(bool IsSuccess, string Message)> AddMajorAsync(Major major)
        {
            if (!ValidationHelper.IsRequired(major.MajorId) || !ValidationHelper.IsRequired(major.MajorName))
                return (false, "Major ID and Name are required.");

            if (await _context.Majors.AnyAsync(m => m.MajorId == major.MajorId))
                return (false, "Major ID already exists.");

            await _context.Majors.AddAsync(major);
            await _context.SaveChangesAsync();
            return (true, "Major added successfully.");
        }

        public async Task<(bool IsSuccess, string Message)> UpdateMajorAsync(Major major)
        {
            if (!ValidationHelper.IsRequired(major.MajorName))
                return (false, "Major Name is required.");

            _context.Majors.Update(major);
            await _context.SaveChangesAsync();
            return (true, "Major updated successfully.");
        }

        public async Task<(bool IsSuccess, string Message)> DeactivateMajorAsync(string majorId)
        {
            var major = await _context.Majors.FindAsync(majorId);
            if (major == null) return (false, "Major not found.");

            var hasActiveClasses = await _context.Classes.AnyAsync(c => c.MajorId == majorId && c.Status == ClassStatus.Active);
            if (hasActiveClasses) return (false, "Cannot deactivate: major has active classes referencing it.");

            major.Status = MajorStatus.Inactive;
            await _context.SaveChangesAsync();
            return (true, "Major deactivated.");
        }

        // Classes
        public async Task<IEnumerable<Class>> GetAllClassesAsync()
        {
            return await _context.Classes
                .AsNoTracking()
                .Include(c => c.Major)
                .Include(c => c.HomeroomLecturer)
                .ToListAsync();
        }

        public async Task<(bool IsSuccess, string Message)> AddClassAsync(Class cls)
        {
            if (!ValidationHelper.IsRequired(cls.ClassId) || !ValidationHelper.IsRequired(cls.ClassName))
                return (false, "Class ID and Name are required.");

            if (await _context.Classes.AnyAsync(c => c.ClassId == cls.ClassId))
                return (false, "Class ID already exists.");

            if (!string.IsNullOrWhiteSpace(cls.LecturerId))
            {
                var lecturer = await _context.Lecturers.FindAsync(cls.LecturerId);
                if (lecturer == null || lecturer.Status != LecturerStatus.Active)
                    return (false, "Homeroom lecturer must be an active lecturer.");
            }

            await _context.Classes.AddAsync(cls);
            await _context.SaveChangesAsync();
            return (true, "Class added successfully.");
        }

        public async Task<(bool IsSuccess, string Message)> UpdateClassAsync(Class cls)
        {
            if (!ValidationHelper.IsRequired(cls.ClassName))
                return (false, "Class Name is required.");

            _context.Classes.Update(cls);
            await _context.SaveChangesAsync();
            return (true, "Class updated successfully.");
        }

        // Lecturers
        public async Task<IEnumerable<Lecturer>> GetAllLecturersAsync(string? searchKeyword = null)
        {
            var query = _context.Lecturers.AsNoTracking().AsQueryable();
            if (!string.IsNullOrWhiteSpace(searchKeyword))
                query = query.Where(l => l.LecturerId.Contains(searchKeyword) || l.FullName.Contains(searchKeyword));
            return await query.ToListAsync();
        }

        public async Task<(bool IsSuccess, string Message)> AddLecturerAsync(Lecturer lecturer)
        {
            if (!ValidationHelper.IsRequired(lecturer.LecturerId) || !ValidationHelper.IsRequired(lecturer.FullName))
                return (false, "Lecturer ID and Name are required.");
            if (!ValidationHelper.IsValidEmail(lecturer.Email))
                return (false, "Email format is invalid.");

            if (await _context.Lecturers.AnyAsync(l => l.LecturerId == lecturer.LecturerId))
                return (false, "Lecturer ID already exists.");

            await _context.Lecturers.AddAsync(lecturer);
            await _context.SaveChangesAsync();
            return (true, "Lecturer added successfully.");
        }

        public async Task<(bool IsSuccess, string Message)> UpdateLecturerAsync(Lecturer lecturer)
        {
            if (!ValidationHelper.IsValidEmail(lecturer.Email))
                return (false, "Email format is invalid.");

            _context.Lecturers.Update(lecturer);
            await _context.SaveChangesAsync();
            return (true, "Lecturer updated successfully.");
        }

        public async Task<(bool IsSuccess, string Message)> DeactivateLecturerAsync(string lecturerId)
        {
            var lecturer = await _context.Lecturers.FindAsync(lecturerId);
            if (lecturer == null) return (false, "Lecturer not found.");

            var isHomeroom = await _context.Classes.AnyAsync(c => c.LecturerId == lecturerId && c.Status == ClassStatus.Active);
            var teachesSection = await _context.CourseSections.AnyAsync(cs => cs.LecturerId == lecturerId && cs.Status == CourseSectionStatus.Opened);
            if (isHomeroom || teachesSection)
                return (false, "Cannot deactivate: lecturer is referenced by an active class or course section.");

            lecturer.Status = LecturerStatus.Inactive;
            await _context.SaveChangesAsync();
            return (true, "Lecturer deactivated.");
        }

        // Class Operations
        public async Task<IEnumerable<Student>> GetStudentsByClassAsync(string classId)
        {
            return await _context.Students
                .AsNoTracking()
                .Where(s => s.ClassId == classId)
                .ToListAsync();
        }

        public async Task<(bool IsSuccess, string Message)> AssignHomeroomLecturerAsync(string classId, string lecturerId)
        {
            var cls = await _context.Classes.FindAsync(classId);
            if (cls == null) return (false, "Class not found.");

            var lecturer = await _context.Lecturers.FindAsync(lecturerId);
            if (lecturer == null || lecturer.Status != LecturerStatus.Active)
                return (false, "Lecturer must be active to be assigned as homeroom.");

            cls.LecturerId = lecturerId;
            await _context.SaveChangesAsync();
            return (true, "Homeroom lecturer assigned.");
        }

        public async Task<(bool IsSuccess, string Message)> AssignStudentToClassAsync(string studentId, string newClassId)
        {
            var student = await _context.Students.FindAsync(studentId);
            if (student == null) return (false, "Student not found.");

            var newClass = await _context.Classes.FindAsync(newClassId);
            if (newClass == null || newClass.Status != ClassStatus.Active)
                return (false, "Target class must exist and be active.");

            student.ClassId = newClassId;
            await _context.SaveChangesAsync();
            return (true, "Student assigned to class.");
        }

        public async Task<(int totalStudents, int totalLecturers, int totalClasses, int totalMajors, int activeStudents, int graduatedStudents)> GetDashboardSummaryAsync()
        {
            var totalStudents = await _context.Students.CountAsync();
            var totalLecturers = await _context.Lecturers.CountAsync();
            var totalClasses = await _context.Classes.CountAsync();
            var totalMajors = await _context.Majors.CountAsync();
            var activeStudents = await _context.Students.CountAsync(s => s.Status == StudentStatus.Studying);
            var graduatedStudents = await _context.Students.CountAsync(s => s.Status == StudentStatus.Graduated);

            return (totalStudents, totalLecturers, totalClasses, totalMajors, activeStudents, graduatedStudents);
        }
    }
}
