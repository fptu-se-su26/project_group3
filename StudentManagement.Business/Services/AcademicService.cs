using Microsoft.EntityFrameworkCore;
using StudentManagement.Business.Interfaces;
using StudentManagement.DataAccess;
using StudentManagement.Domain.Entities;
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
        public async Task<IEnumerable<Major>> GetAllMajorsAsync()
        {
            return await _context.Majors.ToListAsync();
        }

        public async Task AddMajorAsync(Major major)
        {
            await _context.Majors.AddAsync(major);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateMajorAsync(Major major)
        {
            _context.Majors.Update(major);
            await _context.SaveChangesAsync();
        }

        // Classes
        public async Task<IEnumerable<Class>> GetAllClassesAsync()
        {
            return await _context.Classes
                .Include(c => c.Major)
                .Include(c => c.HomeroomLecturer)
                .ToListAsync();
        }

        public async Task AddClassAsync(Class cls)
        {
            await _context.Classes.AddAsync(cls);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateClassAsync(Class cls)
        {
            _context.Classes.Update(cls);
            await _context.SaveChangesAsync();
        }

        // Lecturers
        public async Task<IEnumerable<Lecturer>> GetAllLecturersAsync()
        {
            return await _context.Lecturers.ToListAsync();
        }

        public async Task AddLecturerAsync(Lecturer lecturer)
        {
            await _context.Lecturers.AddAsync(lecturer);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateLecturerAsync(Lecturer lecturer)
        {
            _context.Lecturers.Update(lecturer);
            await _context.SaveChangesAsync();
        }

        // Class Operations
        public async Task<IEnumerable<Student>> GetStudentsByClassAsync(string classId)
        {
            return await _context.Students
                .Where(s => s.ClassId == classId)
                .ToListAsync();
        }

        public async Task AssignHomeroomLecturerAsync(string classId, string lecturerId)
        {
            var cls = await _context.Classes.FindAsync(classId);
            if (cls != null)
            {
                cls.LecturerId = lecturerId;
                await _context.SaveChangesAsync();
            }
        }

        public async Task AssignStudentToClassAsync(string studentId, string newClassId)
        {
            var student = await _context.Students.FindAsync(studentId);
            if (student != null)
            {
                student.ClassId = newClassId;
                await _context.SaveChangesAsync();
            }
        }
    }
}
