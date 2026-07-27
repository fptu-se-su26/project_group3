using StudentManagement.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StudentManagement.Business.Export
{
    public interface IStudentExporter
    {
        Task ExportAsync(string filePath, IEnumerable<Student> students);
    }
}
