using System;

namespace StudentManagement.Business.Export
{
    public enum ExportFormat { Json, Xml }

    public static class StudentExporterFactory
    {
        public static IStudentExporter Create(ExportFormat format) => format switch
        {
            ExportFormat.Json => new JsonStudentExporter(),
            ExportFormat.Xml => new XmlStudentExporter(),
            _ => throw new ArgumentOutOfRangeException(nameof(format))
        };
    }
}
