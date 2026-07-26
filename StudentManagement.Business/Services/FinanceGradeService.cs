using Microsoft.EntityFrameworkCore;
using StudentManagement.Business.Interfaces;
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
                .Include(g => g.Registration)
                .ThenInclude(r => r.Student)
                .Where(g => g.Registration.SectionId == sectionId && g.Registration.Status != RegistrationStatus.Cancelled)
                .ToListAsync();
        }

        public async Task UpdateGradeAsync(int gradeId, double? assignment, double? progressTest, double? practical, double? finalExam)
        {
            var grade = await _context.Grades.FindAsync(gradeId);
            if (grade != null)
            {
                grade.Assignment = assignment;
                grade.ProgressTest = progressTest;
                grade.Practical = practical;
                grade.FinalExam = finalExam;

                // Auto calculate GPA (assuming 20% Assignment, 20% PT, 20% Practical, 40% FinalExam)
                double total = 0;
                if (assignment.HasValue) total += assignment.Value * 0.2;
                if (progressTest.HasValue) total += progressTest.Value * 0.2;
                if (practical.HasValue) total += practical.Value * 0.2;
                if (finalExam.HasValue) total += finalExam.Value * 0.4;

                grade.FinalGrade = Math.Round(total, 2);
                grade.Result = (grade.FinalGrade >= 5.0 && finalExam >= 4.0) ? ResultClassification.Pass : ResultClassification.Fail;

                _context.Grades.Update(grade);
                await _context.SaveChangesAsync();
            }
        }

        // Tuition & Finance
        public async Task<IEnumerable<Tuition>> GetAllTuitionsAsync(string? semesterId = null)
        {
            var query = _context.Tuitions
                .Include(t => t.Student)
                .Include(t => t.Semester)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(semesterId))
            {
                query = query.Where(t => t.SemesterId == semesterId);
            }

            return await query.ToListAsync();
        }

        public async Task GenerateTuitionForSemesterAsync(string semesterId)
        {
            // Find all registered students for the semester
            var registrations = await _context.Registrations
                .Include(r => r.CourseSection)
                .ThenInclude(cs => cs.Subject)
                .Where(r => r.CourseSection.SemesterId == semesterId && r.Status != RegistrationStatus.Cancelled)
                .ToListAsync();

            var studentGroups = registrations.GroupBy(r => r.StudentId);

            foreach (var group in studentGroups)
            {
                var studentId = group.Key;
                var totalCredits = group.Sum(r => r.CourseSection.Subject.Credits);
                
                // Assuming price per credit is fixed at 1,000,000 for simplicity
                decimal pricePerCredit = 1000000m;
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
                    // Update if credits changed
                    existingTuition.TotalCredits = totalCredits;
                    existingTuition.Amount = totalAmount;
                    existingTuition.Status = (existingTuition.PaidAmount >= totalAmount) ? TuitionStatus.Paid : (existingTuition.PaidAmount > 0 ? TuitionStatus.Partial : TuitionStatus.Unpaid);
                    _context.Tuitions.Update(existingTuition);
                }
            }
            await _context.SaveChangesAsync();
        }

        public async Task ProcessPaymentAsync(int tuitionId, decimal amount, PaymentMethod method, string? note)
        {
            var tuition = await _context.Tuitions.FindAsync(tuitionId);
            if (tuition != null)
            {
                var payment = new Payment
                {
                    TuitionId = tuitionId,
                    Amount = amount,
                    PaymentDate = DateTime.Now,
                    Method = method,
                    Note = note
                };

                tuition.PaidAmount += amount;
                if (tuition.PaidAmount >= tuition.Amount)
                {
                    tuition.Status = TuitionStatus.Paid;
                }
                else if (tuition.PaidAmount > 0)
                {
                    tuition.Status = TuitionStatus.Partial;
                }

                await _context.Payments.AddAsync(payment);
                _context.Tuitions.Update(tuition);
                await _context.SaveChangesAsync();
            }
        }
    }
}
