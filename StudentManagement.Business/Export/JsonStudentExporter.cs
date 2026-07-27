using StudentManagement.Domain.Entities;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace StudentManagement.Business.Export
{
    public class JsonStudentExporter : IStudentExporter
    {
        public async Task ExportAsync(string filePath, IEnumerable<Student> students)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            var jsonString = JsonSerializer.Serialize(students, options);
            await File.WriteAllTextAsync(filePath, jsonString);
        }
    }
}
