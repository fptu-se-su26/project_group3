using StudentManagement.Business.DTOs;
using StudentManagement.Domain.Entities;
using StudentManagement.Domain.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StudentManagement.Business.Interfaces
{
    public interface IFinanceGradeService
    {
        // Grades
        Task<IEnumerable<Grade>> GetGradesForSectionAsync(string sectionId);
        Task UpdateGradeAsync(int gradeId, double? assignment, double? progressTest, double? practical, double? finalExam);

        // Tuition & Finance
        Task<IEnumerable<Tuition>> GetAllTuitionsAsync(string? semesterId = null);
        Task GenerateTuitionForSemesterAsync(string semesterId);
        Task ProcessPaymentAsync(int tuitionId, decimal amount, PaymentMethod method, string? note);
        Task<(bool IsSuccess, string Message)> UpdateGradeAsync(int gradeId, double? assignment, double? progressTest, double? practical, double? finalExam);

        // Tuition & Finance
        Task<IEnumerable<Tuition>> GetAllTuitionsAsync(string? semesterId = null);
        Task GenerateTuitionForSemesterAsync(string semesterId, decimal pricePerCredit);
        Task<(bool IsSuccess, string Message)> ProcessPaymentAsync(int tuitionId, decimal amount, PaymentMethod method, string? note);

        // Results & Reports
        Task<AcademicResultSummary> GetAcademicResultsAsync(string studentId, string semesterId);
        Task<IEnumerable<ClassReportItem>> GetClassReportAsync();
        Task<IEnumerable<MajorReportItem>> GetMajorReportAsync();
        Task<IEnumerable<ResultReportItem>> GetResultReportAsync(string? semesterId = null);
        Task<IEnumerable<ResultReportItem>> GetFailureReportAsync(string? semesterId = null);
        Task<IEnumerable<UnpaidTuitionReportItem>> GetUnpaidTuitionReportAsync();
        Task<IEnumerable<CollectedTuitionReportItem>> GetCollectedTuitionReportAsync();
    }
}
