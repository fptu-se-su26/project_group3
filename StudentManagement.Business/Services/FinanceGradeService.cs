using Microsoft.EntityFrameworkCore;
using StudentManagement.Business.DTOs;
using StudentManagement.Business.Interfaces;
using StudentManagement.Business.Validators;
using StudentManagement.DataAccess;
using StudentManagement.Domain.Entities;
using StudentManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentManagement.Business.Services
{
    public class FinanceGradeService : IFinanceGradeService
    {
        private readonly AppDbContext _context;

        public FinanceGradeService(AppDbContext context)
        {
            _context = context;
        }

        // Grades
        public async Task<IEnumerable<Grade>> GetGradesForSectionAsync(string sectionId)
        {
            return await _context.Grades
                .AsNoTracking()
                .Include(g => g.Registration)
                .ThenInclude(r => r.Student)
                .Where(g => g.Registration.SectionId == sectionId && g.Registration.Status != RegistrationStatus.Cancelled)
                .ToListAsync();
        }

        public async Task<(bool IsSuccess, string Message)> UpdateGradeAsync(int gradeId, double? assignment, double? progressTest, double? practical, double? finalExam)
        {
            if (!ValidationHelper.IsInRange(assignment, 0, 10) || !ValidationHelper.IsInRange(progressTest, 0, 10)
                || !ValidationHelper.IsInRange(practical, 0, 10) || !ValidationHelper.IsInRange(finalExam, 0, 10))
                return (false, "All scores must be between 0 and 10.");

            var grade = await _context.Grades.FindAsync(gradeId);
            if (grade == null) return (false, "Grade record not found.");

            grade.Assignment = assignment;
            grade.ProgressTest = progressTest;
            grade.Practical = practical;
            grade.FinalExam = finalExam;

            // F26: Calculate final grade using 20% Assignment, 20% Progress Test, 20% Practical, and 40% Final Exam weights
            double total = 0;
            if (assignment.HasValue) total += assignment.Value * 0.2;
            if (progressTest.HasValue) total += progressTest.Value * 0.2;
            if (practical.HasValue) total += practical.Value * 0.2;
            if (finalExam.HasValue) total += finalExam.Value * 0.4;

            grade.FinalGrade = Math.Round(total, 2);
            grade.Result = (grade.FinalGrade >= 5.0) ? ResultClassification.Pass : ResultClassification.Fail;

            await _context.SaveChangesAsync();
            return (true, "Grade saved successfully.");
        }

        // Tuition & Finance
        public async Task<IEnumerable<Tuition>> GetAllTuitionsAsync(string? semesterId = null)
        {
            var query = _context.Tuitions
                .AsNoTracking()
                .Include(t => t.Student)
                .Include(t => t.Semester)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(semesterId))
            {
                query = query.Where(t => t.SemesterId == semesterId);
            }

            return await query.ToListAsync();
        }

        // F28: Manage student tuition (Calculate tuition from registered credits & price per credit; track due date and balance)
        public async Task GenerateTuitionForSemesterAsync(string semesterId, decimal pricePerCredit)
        {
            var registrations = await _context.Registrations
                .AsNoTracking()
                .Include(r => r.CourseSection)
                .ThenInclude(cs => cs.Subject)
                .Where(r => r.CourseSection.SemesterId == semesterId && r.Status != RegistrationStatus.Cancelled)
                .ToListAsync();

            var studentGroups = registrations.GroupBy(r => r.StudentId);

            foreach (var group in studentGroups)
            {
                var studentId = group.Key;
                var totalCredits = group.Sum(r => r.CourseSection.Subject.Credits);
                decimal totalAmount = totalCredits * pricePerCredit;

                var existingTuition = await _context.Tuitions.FirstOrDefaultAsync(t => t.StudentId == studentId && t.SemesterId == semesterId);

                if (existingTuition == null)
                {
                    var tuition = new Tuition
                    {
                        StudentId = studentId,
                        SemesterId = semesterId,
                        TotalCredits = totalCredits,
                        PricePerCredit = pricePerCredit,
                        Amount = totalAmount,
                        PaidAmount = 0,
                        DueDate = DateTime.Now.AddMonths(1),
                        Status = TuitionStatus.Unpaid
                    };
                    await _context.Tuitions.AddAsync(tuition);
                }
                else
                {
                    existingTuition.TotalCredits = totalCredits;
                    existingTuition.PricePerCredit = pricePerCredit;
                    existingTuition.Amount = totalAmount;
                    existingTuition.Status = (existingTuition.PaidAmount >= totalAmount) ? TuitionStatus.Paid : (existingTuition.PaidAmount > 0 ? TuitionStatus.Partial : TuitionStatus.Unpaid);
                    _context.Tuitions.Update(existingTuition);
                }
            }
            await _context.SaveChangesAsync();
        }

        public async Task<(bool IsSuccess, string Message)> ProcessPaymentAsync(int tuitionId, decimal amount, PaymentMethod method, string? note)
        {
            var tuition = await _context.Tuitions.FindAsync(tuitionId);
            if (tuition == null) return (false, "Tuition record not found.");

            if (amount <= 0) return (false, "Payment amount must be greater than 0.");

            var remaining = tuition.Amount - tuition.PaidAmount;
            if (amount > remaining) return (false, $"Payment amount exceeds outstanding balance ({remaining:N0}).");

            var payment = new Payment
            {
                TuitionId = tuitionId,
                Amount = amount,
                PaymentDate = DateTime.Now,
                Method = method,
                Note = note
            };

            tuition.PaidAmount += amount;
            tuition.Status = tuition.PaidAmount >= tuition.Amount ? TuitionStatus.Paid
                : tuition.PaidAmount > 0 ? TuitionStatus.Partial
                : TuitionStatus.Unpaid;

            await _context.Payments.AddAsync(payment);
            _context.Tuitions.Update(tuition);
            await _context.SaveChangesAsync();

            return (true, "Payment processed successfully.");
        }

        // F27: View Academic Results (Show semester results, completed credits, average score, passed/failed subjects)
        public async Task<AcademicResultSummary> GetAcademicResultsAsync(string studentId, string semesterId)
        {
            var registrations = await _context.Registrations
                .AsNoTracking()
                .Include(r => r.CourseSection).ThenInclude(cs => cs.Subject)
                .Include(r => r.Grade)
                .Where(r => r.StudentId == studentId && r.CourseSection.SemesterId == semesterId && r.Status != RegistrationStatus.Cancelled)
                .ToListAsync();

            var summary = new AcademicResultSummary { StudentId = studentId, SemesterId = semesterId };
            var gradedScores = new List<double>();

            foreach (var r in registrations)
            {
                if (r.Grade?.FinalGrade == null) continue;

                gradedScores.Add(r.Grade.FinalGrade.Value);
                if (r.Grade.Result == ResultClassification.Pass)
                {
                    summary.PassedSubjects.Add(r.CourseSection.Subject.SubjectName);
                    summary.CompletedCredits += r.CourseSection.Subject.Credits;
                }
                else
                {
                    summary.FailedSubjects.Add(r.CourseSection.Subject.SubjectName);
                }
            }

            summary.AverageScore = gradedScores.Any() ? Math.Round(gradedScores.Average(), 2) : 0;
            return summary;
        }

        public async Task<IEnumerable<ClassReportItem>> GetClassReportAsync()
        {
            return await _context.Classes
                .Include(c => c.Major)
                .Select(c => new ClassReportItem
                {
                    ClassId = c.ClassId,
                    ClassName = c.ClassName,
                    MajorName = c.Major.MajorName,
                    TotalStudents = c.Students.Count
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<MajorReportItem>> GetMajorReportAsync()
        {
            return await _context.Majors
                .Select(m => new MajorReportItem
                {
                    MajorId = m.MajorId,
                    MajorName = m.MajorName,
                    TotalClasses = m.Classes.Count,
                    TotalStudents = m.Classes.SelectMany(c => c.Students).Count()
                })
                .ToListAsync();
        }

        private IQueryable<Registration> GradedRegistrationsQuery(string? semesterId)
        {
            var query = _context.Registrations
                .AsNoTracking()
                .Include(r => r.Student)
                .Include(r => r.CourseSection).ThenInclude(cs => cs.Subject)
                .Include(r => r.Grade)
                .Where(r => r.Status != RegistrationStatus.Cancelled && r.Grade != null && r.Grade.Result != null);

            if (!string.IsNullOrWhiteSpace(semesterId))
                query = query.Where(r => r.CourseSection.SemesterId == semesterId);

            return query;
        }

        public async Task<IEnumerable<ResultReportItem>> GetResultReportAsync(string? semesterId = null)
        {
            var registrations = await GradedRegistrationsQuery(semesterId).ToListAsync();
            return registrations.Select(r => new ResultReportItem
            {
                StudentId = r.StudentId,
                StudentName = r.Student.FullName,
                SubjectId = r.CourseSection.SubjectId,
                SubjectName = r.CourseSection.Subject.SubjectName,
                FinalGrade = r.Grade!.FinalGrade,
                Result = r.Grade!.Result.ToString()!
            });
        }

        public async Task<IEnumerable<ResultReportItem>> GetFailureReportAsync(string? semesterId = null)
        {
            var registrations = await GradedRegistrationsQuery(semesterId)
                .Where(r => r.Grade!.Result == ResultClassification.Fail)
                .ToListAsync();

            return registrations.Select(r => new ResultReportItem
            {
                StudentId = r.StudentId,
                StudentName = r.Student.FullName,
                SubjectId = r.CourseSection.SubjectId,
                SubjectName = r.CourseSection.Subject.SubjectName,
                FinalGrade = r.Grade!.FinalGrade,
                Result = r.Grade!.Result.ToString()!
            });
        }

        public async Task<IEnumerable<UnpaidTuitionReportItem>> GetUnpaidTuitionReportAsync()
        {
            return await _context.Tuitions
                .Include(t => t.Student)
                .Where(t => t.Status != TuitionStatus.Paid)
                .Select(t => new UnpaidTuitionReportItem
                {
                    StudentId = t.StudentId,
                    StudentName = t.Student.FullName,
                    SemesterId = t.SemesterId,
                    Amount = t.Amount,
                    PaidAmount = t.PaidAmount
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<CollectedTuitionReportItem>> GetCollectedTuitionReportAsync()
        {
            return await _context.Payments
                .Include(p => p.Tuition)
                .GroupBy(p => p.Tuition.SemesterId)
                .Select(g => new CollectedTuitionReportItem
                {
                    SemesterId = g.Key,
                    PaymentCount = g.Count(),
                    TotalCollected = g.Sum(p => p.Amount)
                })
                .ToListAsync();
        }
    }
}
