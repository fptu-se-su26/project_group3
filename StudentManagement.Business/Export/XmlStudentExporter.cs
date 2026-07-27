using StudentManagement.Domain.Entities;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace StudentManagement.Business.Export
{
    public class XmlStudentExporter : IStudentExporter
    {
        public Task ExportAsync(string filePath, IEnumerable<Student> students)
        {
            var serializer = new XmlSerializer(typeof(List<Student>));
            using var stream = new FileStream(filePath, FileMode.Create);
            serializer.Serialize(stream, students.ToList());
            return Task.CompletedTask;
        }
    }
}
