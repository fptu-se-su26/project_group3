using StudentManagement.Business.Services;
using StudentManagement.Domain.Entities;
using StudentManagement.Domain.Enums;
using System.Threading.Tasks;
using Xunit;

namespace StudentManagement.Tests
{
    public class GradeCalculationTests
    {
        private static async Task<(FinanceGradeService service, int gradeId)> SeedAsync(DataAccess.AppDbContext context)
        {
            var registration = new Registration { StudentId = "SE001", SectionId = "CS101-SP24", RegistrationDate = System.DateTime.Now, Status = RegistrationStatus.Registered };
            context.Registrations.Add(registration);
            await context.SaveChangesAsync();

            var grade = new Grade { RegistrationId = registration.RegistrationId };
            context.Grades.Add(grade);
            await context.SaveChangesAsync();

            return (new FinanceGradeService(context), grade.GradeId);
        }

        [Fact]
        public async Task UpdateGradeAsync_CalculatesWeightedFinalGrade()
        {
            using var context = TestDbFactory.CreateContext();
            var (service, gradeId) = await SeedAsync(context);

            // 8*0.2 + 7*0.2 + 9*0.2 + 6*0.4 = 1.6+1.4+1.8+2.4 = 7.2
            var (success, _) = await service.UpdateGradeAsync(gradeId, 8, 7, 9, 6);

            Assert.True(success);
            var grade = await context.Grades.FindAsync(gradeId);
            Assert.Equal(7.2, grade!.FinalGrade);
            Assert.Equal(ResultClassification.Pass, grade.Result);
        }

        [Fact]
        public async Task UpdateGradeAsync_BelowPassingThreshold_IsFail()
        {
            using var context = TestDbFactory.CreateContext();
            var (service, gradeId) = await SeedAsync(context);

            await service.UpdateGradeAsync(gradeId, 3, 3, 3, 3);

            var grade = await context.Grades.FindAsync(gradeId);
            Assert.Equal(ResultClassification.Fail, grade!.Result);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(10.5)]
        public async Task UpdateGradeAsync_ScoreOutOfRange_IsRejected(double invalidScore)
        {
            using var context = TestDbFactory.CreateContext();
            var (service, gradeId) = await SeedAsync(context);

            var (success, message) = await service.UpdateGradeAsync(gradeId, invalidScore, 5, 5, 5);

            Assert.False(success);
            Assert.Contains("between 0 and 10", message);
        }
    }
}
