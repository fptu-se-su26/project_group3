using System.Collections.Generic;

namespace StudentManagement.Business.DTOs
{
    public class ClassReportItem
    {
        public string ClassId { get; set; } = null!;
        public string ClassName { get; set; } = null!;
        public string MajorName { get; set; } = null!;
        public int TotalStudents { get; set; }
    }

    public class MajorReportItem
    {
        public string MajorId { get; set; } = null!;
        public string MajorName { get; set; } = null!;
        public int TotalClasses { get; set; }
        public int TotalStudents { get; set; }
    }

    public class ResultReportItem
    {
        public string StudentId { get; set; } = null!;
        public string StudentName { get; set; } = null!;
        public string SubjectId { get; set; } = null!;
        public string SubjectName { get; set; } = null!;
        public double? FinalGrade { get; set; }
        public string Result { get; set; } = null!;
    }

    public class UnpaidTuitionReportItem
    {
        public string StudentId { get; set; } = null!;
        public string StudentName { get; set; } = null!;
        public string SemesterId { get; set; } = null!;
        public decimal Amount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal Outstanding => Amount - PaidAmount;
    }

    public class CollectedTuitionReportItem
    {
        public string SemesterId { get; set; } = null!;
        public int PaymentCount { get; set; }
        public decimal TotalCollected { get; set; }
    }

    public class AcademicResultSummary
    {
        public string StudentId { get; set; } = null!;
        public string SemesterId { get; set; } = null!;
        public int CompletedCredits { get; set; }
        public double AverageScore { get; set; }
        public List<string> PassedSubjects { get; set; } = new();
        public List<string> FailedSubjects { get; set; } = new();
    }
}
