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
    }
}
