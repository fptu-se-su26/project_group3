using Microsoft.EntityFrameworkCore;
using StudentManagement.Business.Security;
using StudentManagement.DataAccess;
using StudentManagement.Domain.Entities;
using StudentManagement.Domain.Enums;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace StudentManagement.Business.Seeding
{
    /// <summary>
    /// Seeds realistic sample data (3 accounts per non-Admin role, with related
    /// academic/finance records) so every role can be exercised end-to-end.
    /// Runs once: skipped if seed accounts already exist.
    /// </summary>
    public static class DevDataSeeder
    {
        public const string DefaultPassword = "Pass@123";

        public static async Task SeedAsync(AppDbContext context)
        {
            if (await context.Users.AnyAsync(u => u.Username == "staff01"))
                return; // already seeded

            // ---- Academic reference data ----
            var majors = new[]
            {
                new Major { MajorId = "IT", MajorName = "Information Technology", Status = MajorStatus.Active },
                new Major { MajorId = "BA", MajorName = "Business Administration", Status = MajorStatus.Active },
                new Major { MajorId = "EE", MajorName = "Electrical Engineering", Status = MajorStatus.Active },
            };
            await context.Majors.AddRangeAsync(majors);

            var classes = new[]
            {
                new Class { ClassId = "K17A", ClassName = "K17A - IT", MajorId = "IT", AcademicYear = "2024", Status = ClassStatus.Active },
                new Class { ClassId = "K17B", ClassName = "K17B - BA", MajorId = "BA", AcademicYear = "2024", Status = ClassStatus.Active },
                new Class { ClassId = "K17C", ClassName = "K17C - EE", MajorId = "EE", AcademicYear = "2024", Status = ClassStatus.Active },
            };
            await context.Classes.AddRangeAsync(classes);

            var subjects = new[]
            {
                new Subject { SubjectId = "SUB01", SubjectName = "Introduction to Programming", Credits = 3, Status = SubjectStatus.Active },
                new Subject { SubjectId = "SUB02", SubjectName = "Database Systems", Credits = 4, Status = SubjectStatus.Active },
                new Subject { SubjectId = "SUB03", SubjectName = "Computer Networking", Credits = 3, Status = SubjectStatus.Active },
            };
            await context.Subjects.AddRangeAsync(subjects);

            var semester = new Semester { SemesterId = "SP24", SemesterName = "Spring 2024", AcademicYear = "2024", StartDate = new DateTime(2024, 1, 15), EndDate = new DateTime(2024, 5, 15) };
            await context.Semesters.AddAsync(semester);

            await context.SaveChangesAsync();

            // ---- Lecturer accounts (Role 3) + Lecturer profiles ----
            var lecturerIds = new[] { "gv001", "gv002", "gv003" };
            var lecturerNames = new[] { "Nguyen Van A", "Tran Thi B", "Le Van C" };
            for (int i = 0; i < 3; i++)
            {
                await context.Lecturers.AddAsync(new Lecturer
                {
                    LecturerId = lecturerIds[i],
                    FullName = lecturerNames[i],
                    Email = $"{lecturerIds[i]}@sms.edu.vn",
                    Status = LecturerStatus.Active
                });
                await context.Users.AddAsync(new User
                {
                    Username = lecturerIds[i],
                    PasswordHash = PasswordHasher.Hash(DefaultPassword),
                    RoleId = 3,
                    Status = true
                });
            }
            await context.SaveChangesAsync();

            classes[0].LecturerId = lecturerIds[0];
            classes[1].LecturerId = lecturerIds[1];
            classes[2].LecturerId = lecturerIds[2];

            // ---- Academic Staff accounts (Role 2) ----
            foreach (var username in new[] { "staff01", "staff02", "staff03" })
            {
                await context.Users.AddAsync(new User
                {
                    Username = username,
                    PasswordHash = PasswordHasher.Hash(DefaultPassword),
                    RoleId = 2,
                    Status = true
                });
            }

            // ---- Accountant accounts (Role 4) ----
            foreach (var username in new[] { "ketoan01", "ketoan02", "ketoan03" })
            {
                await context.Users.AddAsync(new User
                {
                    Username = username,
                    PasswordHash = PasswordHasher.Hash(DefaultPassword),
                    RoleId = 4,
                    Status = true
                });
            }
            await context.SaveChangesAsync();

            // ---- Course sections, one per lecturer/subject ----
            var sections = new[]
            {
                new CourseSection { SectionId = "SEC01", SubjectId = "SUB01", LecturerId = lecturerIds[0], SemesterId = "SP24", Room = "P101", DayOfWeek = "Monday", StartTime = new TimeSpan(7, 0, 0), EndTime = new TimeSpan(9, 0, 0), Capacity = 40, Status = CourseSectionStatus.Opened },
                new CourseSection { SectionId = "SEC02", SubjectId = "SUB02", LecturerId = lecturerIds[1], SemesterId = "SP24", Room = "P102", DayOfWeek = "Tuesday", StartTime = new TimeSpan(9, 0, 0), EndTime = new TimeSpan(11, 0, 0), Capacity = 40, Status = CourseSectionStatus.Opened },
                new CourseSection { SectionId = "SEC03", SubjectId = "SUB03", LecturerId = lecturerIds[2], SemesterId = "SP24", Room = "P103", DayOfWeek = "Wednesday", StartTime = new TimeSpan(13, 0, 0), EndTime = new TimeSpan(15, 0, 0), Capacity = 40, Status = CourseSectionStatus.Opened },
            };
            await context.CourseSections.AddRangeAsync(sections);
            await context.SaveChangesAsync();

            // ---- Student accounts (Role 5) + Student profiles ----
            var studentIds = new[] { "SV001", "SV002", "SV003" };
            var studentNames = new[] { "Pham Thi D", "Hoang Van E", "Vo Thi F" };
            var studentClasses = new[] { "K17A", "K17B", "K17C" };
            for (int i = 0; i < 3; i++)
            {
                await context.Students.AddAsync(new Student
                {
                    StudentId = studentIds[i],
                    FullName = studentNames[i],
                    DateOfBirth = new DateTime(2004, 1, 1).AddMonths(i),
                    Gender = i % 2 == 0 ? "Female" : "Male",
                    Email = $"{studentIds[i].ToLower()}@sms.edu.vn",
                    ClassId = studentClasses[i],
                    EnrollmentDate = new DateTime(2024, 1, 10),
                    Status = StudentStatus.Studying
                });
                await context.Users.AddAsync(new User
                {
                    Username = studentIds[i],
                    PasswordHash = PasswordHasher.Hash(DefaultPassword),
                    RoleId = 5,
                    Status = true
                });
            }
            await context.SaveChangesAsync();

            // ---- Registrations: each student takes all 3 sections ----
            var registrations = new System.Collections.Generic.List<Registration>();
            foreach (var studentId in studentIds)
            {
                foreach (var section in sections)
                {
                    registrations.Add(new Registration
                    {
                        StudentId = studentId,
                        SectionId = section.SectionId,
                        RegistrationDate = new DateTime(2024, 1, 12),
                        Status = RegistrationStatus.Registered
                    });
                }
            }
            await context.Registrations.AddRangeAsync(registrations);
            await context.SaveChangesAsync();

            // ---- Grades: enter scores for SEC01 and SEC02 (leave SEC03 ungraded) ----
            // SV001: strong pass, SV002: borderline pass, SV003: fail -> gives Reports real variety.
            var gradeInputs = new (string StudentId, string SectionId, double A, double P, double Pr, double F)[]
            {
                ("SV001", "SEC01", 9, 8, 9, 8.5),
                ("SV002", "SEC01", 7, 6, 7, 6),
                ("SV003", "SEC01", 4, 3, 5, 3),
                ("SV001", "SEC02", 8, 8, 8, 7.5),
                ("SV002", "SEC02", 6, 5, 6, 5.5),
                ("SV003", "SEC02", 3, 4, 4, 2.5),
            };

            foreach (var g in gradeInputs)
            {
                var registration = registrations.First(r => r.StudentId == g.StudentId && r.SectionId == g.SectionId);
                double final = Math.Round(g.A * 0.2 + g.P * 0.2 + g.Pr * 0.2 + g.F * 0.4, 2);
                await context.Grades.AddAsync(new Grade
                {
                    RegistrationId = registration.RegistrationId,
                    Assignment = g.A,
                    ProgressTest = g.P,
                    Practical = g.Pr,
                    FinalExam = g.F,
                    FinalGrade = final,
                    Result = final >= 5.0 ? ResultClassification.Pass : ResultClassification.Fail
                });
            }
            await context.SaveChangesAsync();

            // ---- Tuition: 3 credits + 4 credits + 3 credits = 10 credits per student ----
            const decimal pricePerCredit = 1_000_000m;
            var tuitionByStudent = new System.Collections.Generic.Dictionary<string, Tuition>();
            foreach (var studentId in studentIds)
            {
                var totalCredits = subjects.Sum(s => s.Credits); // enrolled in all 3 sections/subjects
                var tuition = new Tuition
                {
                    StudentId = studentId,
                    SemesterId = "SP24",
                    TotalCredits = totalCredits,
                    PricePerCredit = pricePerCredit,
                    Amount = totalCredits * pricePerCredit,
                    PaidAmount = 0,
                    DueDate = new DateTime(2024, 3, 1),
                    Status = TuitionStatus.Unpaid
                };
                tuitionByStudent[studentId] = tuition;
                await context.Tuitions.AddAsync(tuition);
            }
            await context.SaveChangesAsync();

            // SV001: fully paid, SV002: partially paid, SV003: unpaid -> real variety for Finance reports.
            var sv001Tuition = tuitionByStudent["SV001"];
            sv001Tuition.PaidAmount = sv001Tuition.Amount;
            sv001Tuition.Status = TuitionStatus.Paid;
            await context.Payments.AddAsync(new Payment { TuitionId = sv001Tuition.TuitionId, Amount = sv001Tuition.Amount, PaymentDate = new DateTime(2024, 2, 1), Method = PaymentMethod.BankTransfer, Note = "Full payment" });

            var sv002Tuition = tuitionByStudent["SV002"];
            var partial = sv002Tuition.Amount / 2;
            sv002Tuition.PaidAmount = partial;
            sv002Tuition.Status = TuitionStatus.Partial;
            await context.Payments.AddAsync(new Payment { TuitionId = sv002Tuition.TuitionId, Amount = partial, PaymentDate = new DateTime(2024, 2, 5), Method = PaymentMethod.Cash, Note = "Partial payment" });

            // SV003 stays fully unpaid.

            await context.SaveChangesAsync();
        }
    }
}
