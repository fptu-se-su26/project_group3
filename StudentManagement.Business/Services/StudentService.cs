using Microsoft.EntityFrameworkCore;
using StudentManagement.Business.Export;
using StudentManagement.Business.Interfaces;
using StudentManagement.Business.Validators;
using StudentManagement.DataAccess;
using StudentManagement.DataAccess.Repositories;
using StudentManagement.Domain.Entities;
using StudentManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace StudentManagement.Business.Services
{
    public class StudentService : IStudentService
    {
        private readonly AppDbContext _context;
        private readonly IBaseRepository<Student> _repository;

        public StudentService(AppDbContext context, IBaseRepository<Student> repository)
        {
            _context = context;
            _repository = repository;
        }

        public async Task<IEnumerable<Student>> GetAllStudentsAsync(string? searchKeyword = null, string? classId = null, string? majorId = null, StudentStatus? status = null)
        {
            var query = _context.Students
                .AsNoTracking()
                .Include(s => s.Class)
                .ThenInclude(c => c.Major)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchKeyword))
            {
                query = query.Where(s => s.StudentId.Contains(searchKeyword) || s.FullName.Contains(searchKeyword));
            }

            if (!string.IsNullOrWhiteSpace(classId))
            {
                query = query.Where(s => s.ClassId == classId);
            }

            if (!string.IsNullOrWhiteSpace(majorId))
            {
                query = query.Where(s => s.Class.MajorId == majorId);
            }

            if (status.HasValue)
            {
                query = query.Where(s => s.Status == status.Value);
            }

            return await query.ToListAsync();
        }

        public async Task<Student?> GetStudentByIdAsync(string id)
        {
            return await _context.Students
                .AsNoTracking()
                .Include(s => s.Class)
                .ThenInclude(c => c.Major)
                .Include(s => s.Registrations)
                .ThenInclude(r => r.CourseSection)
                .ThenInclude(cs => cs.Subject)
                .Include(s => s.Tuitions)
                .FirstOrDefaultAsync(s => s.StudentId == id);
        }

        private static (bool IsSuccess, string Message) ValidateStudent(Student student)
        {
            if (!ValidationHelper.IsRequired(student.StudentId) || !ValidationHelper.IsRequired(student.FullName))
                return (false, "Student ID and Full Name are required.");
            if (!ValidationHelper.IsValidEmail(student.Email))
                return (false, "Email format is invalid.");
            if (!ValidationHelper.IsPastOrToday(student.DateOfBirth))
                return (false, "Date of birth cannot be later than today.");
            if (string.IsNullOrWhiteSpace(student.ClassId))
                return (false, "Class is required.");
            return (true, string.Empty);
        }

        public async Task<(bool IsSuccess, string Message)> AddStudentAsync(Student student)
        {
            var validation = ValidateStudent(student);
            if (!validation.IsSuccess) return validation;

            var exists = await _context.Students.AnyAsync(s => s.StudentId == student.StudentId);
            if (exists) return (false, "Student ID already exists.");

            await _repository.AddAsync(student);
            return (true, "Student added successfully.");
        }

        public async Task<(bool IsSuccess, string Message)> UpdateStudentAsync(Student student)
        {
            var validation = ValidateStudent(student);
            if (!validation.IsSuccess) return validation;

            var existing = await _context.Students.FindAsync(student.StudentId);
            if (existing == null) return (false, "Student not found.");

            existing.FullName = student.FullName;
            existing.DateOfBirth = student.DateOfBirth;
            existing.Gender = student.Gender;
            existing.Email = student.Email;
            existing.Phone = student.Phone;
            existing.Address = student.Address;
            existing.ClassId = student.ClassId;
            // Status changes go exclusively through UpdateStudentStatusAsync so the
            // Suspended/DroppedOut reason requirement can never be bypassed here.

            await _repository.UpdateAsync(existing);
            return (true, "Student updated successfully.");
        }

        public async Task<(bool IsSuccess, string Message)> UpdateStudentStatusAsync(string studentId, StudentStatus newStatus, string? reason)
        {
            var student = await _context.Students.FindAsync(studentId);
            if (student == null) return (false, "Student not found.");

            if ((newStatus == StudentStatus.Suspended || newStatus == StudentStatus.DroppedOut) && string.IsNullOrWhiteSpace(reason))
                return (false, "A reason is required for this status change.");

            student.Status = newStatus;
            student.StatusReason = reason;
            await _context.SaveChangesAsync();
            return (true, "Student status updated.");
        }

        public async Task ExportToJsonAsync(string filePath, IEnumerable<Student> students)
        {
            await StudentExporterFactory.Create(ExportFormat.Json).ExportAsync(filePath, students);
        }

        public async Task ExportToXmlAsync(string filePath, IEnumerable<Student> students)
        {
            await StudentExporterFactory.Create(ExportFormat.Xml).ExportAsync(filePath, students);
        }

        public async Task<(int success, int fail, List<string> errors)> ImportFromJsonAsync(string filePath)
        {
            var errors = new List<string>();
            if (!File.Exists(filePath))
            {
                errors.Add("File not found.");
                return (0, 0, errors);
            }

            string jsonString = await File.ReadAllTextAsync(filePath);
            List<Student>? students;
            try
            {
                students = JsonSerializer.Deserialize<List<Student>>(jsonString);
            }
            catch (JsonException ex)
            {
                errors.Add($"Invalid JSON: {ex.Message}");
                return (0, 0, errors);
            }

            if (students == null || !students.Any()) return (0, 0, errors);

            int success = 0;
            int fail = 0;

            foreach (var student in students)
            {
                var validation = ValidateStudent(student);
                if (!validation.IsSuccess)
                {
                    fail++;
                    errors.Add($"{student.StudentId ?? "(no id)"}: {validation.Message}");
                    continue;
                }

                var existing = await _context.Students.AnyAsync(s => s.StudentId == student.StudentId);
                if (existing)
                {
                    fail++;
                    errors.Add($"{student.StudentId}: duplicate ID, skipped.");
                    continue;
                }

                var classExists = await _context.Classes.AnyAsync(c => c.ClassId == student.ClassId);
                if (!classExists)
                {
                    fail++;
                    errors.Add($"{student.StudentId}: class '{student.ClassId}' does not exist.");
                    continue;
                }

                try
                {
                    await _context.Students.AddAsync(student);
                    await _context.SaveChangesAsync();
                    success++;
                }
                catch (Exception ex)
                {
                    fail++;
                    errors.Add($"{student.StudentId}: {ex.Message}");
                }
            }
            return (success, fail, errors);
        }
    }
}
