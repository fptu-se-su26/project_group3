using StudentManagement.Business.Services;
using StudentManagement.Domain.Entities;
using StudentManagement.Domain.Enums;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace StudentManagement.Tests
{
    public class TuitionTests
    {
        private static async Task<DataAccess.AppDbContext> SeedRegistrationAsync(int credits)
        {
            var context = TestDbFactory.CreateContext();

            context.Majors.Add(new Major { MajorId = "IT", MajorName = "Information Technology", Status = MajorStatus.Active });
            context.Classes.Add(new Class { ClassId = "K1", ClassName = "K1", MajorId = "IT", AcademicYear = "2024", Status = ClassStatus.Active });
            context.Students.Add(new Student { StudentId = "SE001", FullName = "Test Student", Email = "a@b.com", Gender = "M", ClassId = "K1", DateOfBirth = System.DateTime.Now.AddYears(-20), EnrollmentDate = System.DateTime.Now, Status = StudentStatus.Studying });
            context.Subjects.Add(new Subject { SubjectId = "SUB1", SubjectName = "Subject 1", Credits = credits, Status = SubjectStatus.Active });
            context.Semesters.Add(new Semester { SemesterId = "SP24", SemesterName = "Spring 2024", AcademicYear = "2024", StartDate = System.DateTime.Now, EndDate = System.DateTime.Now.AddMonths(4) });
            context.CourseSections.Add(new CourseSection { SectionId = "SEC1", SubjectId = "SUB1", SemesterId = "SP24", Room = "R1", DayOfWeek = "Monday", StartTime = System.TimeSpan.FromHours(7), EndTime = System.TimeSpan.FromHours(9), Capacity = 40, Status = CourseSectionStatus.Opened });
            context.Registrations.Add(new Registration { StudentId = "SE001", SectionId = "SEC1", RegistrationDate = System.DateTime.Now, Status = RegistrationStatus.Registered });
            await context.SaveChangesAsync();

            return context;
        }

        [Fact]
        public async Task GenerateTuitionForSemesterAsync_ComputesCreditsTimesPrice()
        {
            using var context = await SeedRegistrationAsync(credits: 3);
            var service = new FinanceGradeService(context);

            await service.GenerateTuitionForSemesterAsync("SP24", 1_500_000m);

            var tuition = context.Tuitions.Single(t => t.StudentId == "SE001");
            Assert.Equal(3, tuition.TotalCredits);
            Assert.Equal(4_500_000m, tuition.Amount);
        }

        [Fact]
        public async Task ProcessPaymentAsync_OverpaymentIsRejected()
        {
            using var context = await SeedRegistrationAsync(credits: 3);
            var service = new FinanceGradeService(context);
            await service.GenerateTuitionForSemesterAsync("SP24", 1_000_000m);
            var tuition = context.Tuitions.Single(t => t.StudentId == "SE001");

            var (success, message) = await service.ProcessPaymentAsync(tuition.TuitionId, tuition.Amount + 1, PaymentMethod.Cash, null);

            Assert.False(success);
            Assert.Contains("exceeds", message);
        }

        [Fact]
        public async Task ProcessPaymentAsync_ValidPayment_UpdatesStatus()
        {
            using var context = await SeedRegistrationAsync(credits: 3);
            var service = new FinanceGradeService(context);
            await service.GenerateTuitionForSemesterAsync("SP24", 1_000_000m);
            var tuition = context.Tuitions.Single(t => t.StudentId == "SE001");

            var (success, _) = await service.ProcessPaymentAsync(tuition.TuitionId, tuition.Amount, PaymentMethod.Cash, null);

            Assert.True(success);
            var updated = context.Tuitions.Single(t => t.StudentId == "SE001");
            Assert.Equal(TuitionStatus.Paid, updated.Status);
        }
    }
}
