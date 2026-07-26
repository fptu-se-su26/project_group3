using Microsoft.EntityFrameworkCore;
using StudentManagement.Business.Interfaces;
using StudentManagement.DataAccess;
using StudentManagement.Domain.Entities;
using StudentManagement.Domain.Enums;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Xml.Serialization;
using System.Threading.Tasks;

namespace StudentManagement.Business.Services
{
    public class StudentService : IStudentService
    {
        private readonly AppDbContext _context;

        public StudentService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Student>> GetAllStudentsAsync(string? searchKeyword = null, string? classId = null, string? majorId = null, StudentStatus? status = null)
        {
            var query = _context.Students
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
                .Include(s => s.Class)
                .ThenInclude(c => c.Major)
                .FirstOrDefaultAsync(s => s.StudentId == id);
        }

        public async Task AddStudentAsync(Student student)
        {
            await _context.Students.AddAsync(student);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateStudentAsync(Student student)
        {
            var existing = await _context.Students.FindAsync(student.StudentId);
            if (existing != null)
            {
                existing.FullName = student.FullName;
                existing.DateOfBirth = student.DateOfBirth;
                existing.Gender = student.Gender;
                existing.Email = student.Email;
                existing.Phone = student.Phone;
                existing.Address = student.Address;
                existing.ClassId = student.ClassId;
                existing.Status = student.Status;
                
                _context.Students.Update(existing);
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateStudentStatusAsync(string studentId, StudentStatus newStatus)
        {
            var student = await _context.Students.FindAsync(studentId);
            if (student != null)
            {
                student.Status = newStatus;
                await _context.SaveChangesAsync();
            }
        }

        public async Task ExportToJsonAsync(string filePath, IEnumerable<Student> students)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            var jsonString = JsonSerializer.Serialize(students, options);
            await File.WriteAllTextAsync(filePath, jsonString);
        }

        public async Task ExportToXmlAsync(string filePath, IEnumerable<Student> students)
        {
            var serializer = new XmlSerializer(typeof(List<Student>));
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                serializer.Serialize(stream, students.ToList());
            }
            await Task.CompletedTask;
        }

        public async Task<(int success, int fail)> ImportFromJsonAsync(string filePath)
        {
            if (!File.Exists(filePath)) return (0, 0);

            string jsonString = await File.ReadAllTextAsync(filePath);
            var students = JsonSerializer.Deserialize<List<Student>>(jsonString);
            
            if (students == null || !students.Any()) return (0, 0);

            int success = 0;
            int fail = 0;

            foreach (var student in students)
            {
                var existing = await _context.Students.FindAsync(student.StudentId);
                if (existing == null)
                {
                    try
                    {
                        await _context.Students.AddAsync(student);
                        await _context.SaveChangesAsync();
                        success++;
                    }
                    catch
                    {
                        fail++;
                    }
                }
                else
                {
                    fail++; // Skip duplicates
                }
            }
            return (success, fail);
        }
    }
}
