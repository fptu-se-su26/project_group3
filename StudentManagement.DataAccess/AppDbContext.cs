using Microsoft.EntityFrameworkCore;
using StudentManagement.Domain.Entities;
using StudentManagement.DataAccess.Configurations;

namespace StudentManagement.DataAccess
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public AppDbContext() { } // for design time
        
        public DbSet<Role> Roles { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Major> Majors { get; set; } = null!;
        public DbSet<Lecturer> Lecturers { get; set; } = null!;
        public DbSet<Class> Classes { get; set; } = null!;
        public DbSet<Student> Students { get; set; } = null!;
        public DbSet<Subject> Subjects { get; set; } = null!;
        public DbSet<Semester> Semesters { get; set; } = null!;
        public DbSet<CourseSection> CourseSections { get; set; } = null!;
        public DbSet<Registration> Registrations { get; set; } = null!;
        public DbSet<Grade> Grades { get; set; } = null!;
        public DbSet<Tuition> Tuitions { get; set; } = null!;
        public DbSet<Payment> Payments { get; set; } = null!;
        public DbSet<AuditLog> AuditLogs { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=(local);Database=StudentManagementDB;Trusted_Connection=True;TrustServerCertificate=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}
