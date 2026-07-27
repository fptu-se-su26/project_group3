using StudentManagement.Business.Services;
using StudentManagement.DataAccess;
using StudentManagement.Domain.Entities;
using StudentManagement.Domain.Enums;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace StudentManagement.Tests
{
    public class RegistrationTests
    {
        private static async Task<AppDbContext> SeedSectionAsync(int capacity)
        {
            var context = TestDbFactory.CreateContext();
            context.Subjects.Add(new Subject { SubjectId = "SUB1", SubjectName = "Subject 1", Credits = 3, Status = SubjectStatus.Active });
            context.Semesters.Add(new Semester { SemesterId = "SP24", SemesterName = "Spring 2024", AcademicYear = "2024", StartDate = System.DateTime.Now, EndDate = System.DateTime.Now.AddMonths(4) });
            context.CourseSections.Add(new CourseSection { SectionId = "SEC1", SubjectId = "SUB1", SemesterId = "SP24", Room = "R1", DayOfWeek = "Monday", StartTime = System.TimeSpan.FromHours(7), EndTime = System.TimeSpan.FromHours(9), Capacity = capacity, Status = CourseSectionStatus.Opened });
            await context.SaveChangesAsync();
            return context;
        }

        [Fact]
        public async Task RegisterCourseAsync_DuplicateRegistration_IsRejected()
        {
            using var context = await SeedSectionAsync(capacity: 10);
            var service = new CourseService(context);

            await service.RegisterCourseAsync("SE001", "SEC1");
            var (success, message) = await service.RegisterCourseAsync("SE001", "SEC1");

            Assert.False(success);
            Assert.Contains("already registered", message);
        }

        [Fact]
        public async Task RegisterCourseAsync_SectionFull_IsRejected()
        {
            using var context = await SeedSectionAsync(capacity: 1);
            var service = new CourseService(context);

            await service.RegisterCourseAsync("SE001", "SEC1");
            var (success, message) = await service.RegisterCourseAsync("SE002", "SEC1");

            Assert.False(success);
            Assert.Contains("full", message);
        }

        [Fact]
        public async Task CancelRegistrationAsync_AfterGradeEntered_IsRejected()
        {
            using var context = await SeedSectionAsync(capacity: 10);
            var service = new CourseService(context);
            await service.RegisterCourseAsync("SE001", "SEC1");

            var registration = context.Registrations.Single();
            context.Grades.Add(new Grade { RegistrationId = registration.RegistrationId, Assignment = 8 });
            await context.SaveChangesAsync();

            var (success, message) = await service.CancelRegistrationAsync(registration.RegistrationId);

            Assert.False(success);
            Assert.Contains("grades have already been entered", message);
        }

        [Fact]
        public async Task CancelRegistrationAsync_NoGrade_Succeeds()
        {
            using var context = await SeedSectionAsync(capacity: 10);
            var service = new CourseService(context);
            await service.RegisterCourseAsync("SE001", "SEC1");
            var registration = context.Registrations.Single();

            var (success, _) = await service.CancelRegistrationAsync(registration.RegistrationId);

            Assert.True(success);
            Assert.Equal(RegistrationStatus.Cancelled, context.Registrations.Single().Status);
        }
    }
}
