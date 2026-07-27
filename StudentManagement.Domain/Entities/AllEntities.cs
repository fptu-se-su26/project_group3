using System;
using System.Collections.Generic;
using StudentManagement.Domain.Enums;

namespace StudentManagement.Domain.Entities
{
    public class Role {
        public int RoleId { get; set; }
        public string RoleName { get; set; } = null!;
        public ICollection<User> Users { get; set; } = new List<User>();
    }

    public class User {
        public int UserId { get; set; }
        public string Username { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public int RoleId { get; set; }
        public bool Status { get; set; } // active/locked
        public Role Role { get; set; } = null!;
    }

    public class Major {
        public string MajorId { get; set; } = null!;
        public string MajorName { get; set; } = null!;
        public string? Description { get; set; }
        public MajorStatus Status { get; set; }
        public ICollection<Class> Classes { get; set; } = new List<Class>();
    }

    public class Lecturer {
        public string LecturerId { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Phone { get; set; }
        public LecturerStatus Status { get; set; }
        public ICollection<Class> HomeroomClasses { get; set; } = new List<Class>();
        public ICollection<CourseSection> CourseSections { get; set; } = new List<CourseSection>();
    }

    public class Class {
        public string ClassId { get; set; } = null!;
        public string ClassName { get; set; } = null!;
        public string MajorId { get; set; } = null!;
        public string? LecturerId { get; set; }
        public string AcademicYear { get; set; } = null!;
        public ClassStatus Status { get; set; }
        public Major Major { get; set; } = null!;
        public Lecturer? HomeroomLecturer { get; set; }
        public ICollection<Student> Students { get; set; } = new List<Student>();
    }

    public class Student {
        public string StudentId { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string ClassId { get; set; } = null!;
        public DateTime EnrollmentDate { get; set; }
        public StudentStatus Status { get; set; }
        public string? StatusReason { get; set; }
        public Class Class { get; set; } = null!;
        public ICollection<Registration> Registrations { get; set; } = new List<Registration>();
        public ICollection<Tuition> Tuitions { get; set; } = new List<Tuition>();
    }

    public class Subject {
        public string SubjectId { get; set; } = null!;
        public string SubjectName { get; set; } = null!;
        public int Credits { get; set; }
        public string? Description { get; set; }
        public SubjectStatus Status { get; set; }
        public ICollection<CourseSection> CourseSections { get; set; } = new List<CourseSection>();
    }

    public class Semester {
        public string SemesterId { get; set; } = null!;
        public string SemesterName { get; set; } = null!;
        public string AcademicYear { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public ICollection<CourseSection> CourseSections { get; set; } = new List<CourseSection>();
        public ICollection<Tuition> Tuitions { get; set; } = new List<Tuition>();
    }

    public class CourseSection {
        public string SectionId { get; set; } = null!;
        public string SubjectId { get; set; } = null!;
        public string? LecturerId { get; set; }
        public string SemesterId { get; set; } = null!;
        public string Room { get; set; } = null!;
        public string DayOfWeek { get; set; } = null!;
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int Capacity { get; set; }
        public CourseSectionStatus Status { get; set; }
        public Subject Subject { get; set; } = null!;
        public Lecturer? Lecturer { get; set; }
        public Semester Semester { get; set; } = null!;
        public ICollection<Registration> Registrations { get; set; } = new List<Registration>();
    }

    public class Registration {
        public int RegistrationId { get; set; }
        public string StudentId { get; set; } = null!;
        public string SectionId { get; set; } = null!;
        public DateTime RegistrationDate { get; set; }
        public RegistrationStatus Status { get; set; }
        public Student Student { get; set; } = null!;
        public CourseSection CourseSection { get; set; } = null!;
        public Grade? Grade { get; set; }
    }

    public class Grade {
        public int GradeId { get; set; }
        public int RegistrationId { get; set; }
        public double? Assignment { get; set; }
        public double? ProgressTest { get; set; }
        public double? Practical { get; set; }
        public double? FinalExam { get; set; }
        public double? FinalGrade { get; set; }
        public ResultClassification? Result { get; set; }
        public Registration Registration { get; set; } = null!;
    }

    public class Tuition {
        public int TuitionId { get; set; }
        public string StudentId { get; set; } = null!;
        public string SemesterId { get; set; } = null!;
        public int TotalCredits { get; set; }
        public decimal PricePerCredit { get; set; }
        public decimal Amount { get; set; }
        public decimal PaidAmount { get; set; }
        public DateTime DueDate { get; set; }
        public TuitionStatus Status { get; set; }
        public Student Student { get; set; } = null!;
        public Semester Semester { get; set; } = null!;
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }

    public class Payment {
        public int PaymentId { get; set; }
        public int TuitionId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public PaymentMethod Method { get; set; }
        public string? Note { get; set; }
        public Tuition Tuition { get; set; } = null!;
    }

    public class AuditLog {
        public int LogId { get; set; }
        public int UserId { get; set; }
        public string Action { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public User User { get; set; } = null!;
    }
}
