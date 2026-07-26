using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudentManagement.Domain.Entities;

namespace StudentManagement.DataAccess.Configurations
{
    public class RoleConfiguration : IEntityTypeConfiguration<Role> {
        public void Configure(EntityTypeBuilder<Role> builder) {
            builder.HasKey(e => e.RoleId);
            builder.Property(e => e.RoleName).IsRequired().HasMaxLength(50);
        }
    }

    public class UserConfiguration : IEntityTypeConfiguration<User> {
        public void Configure(EntityTypeBuilder<User> builder) {
            builder.HasKey(e => e.UserId);
            builder.HasIndex(e => e.Username).IsUnique();
            builder.Property(e => e.Username).IsRequired().HasMaxLength(50);
            builder.Property(e => e.PasswordHash).IsRequired();
            builder.HasOne(d => d.Role).WithMany(p => p.Users).HasForeignKey(d => d.RoleId).OnDelete(DeleteBehavior.Restrict);
        }
    }

    public class MajorConfiguration : IEntityTypeConfiguration<Major> {
        public void Configure(EntityTypeBuilder<Major> builder) {
            builder.HasKey(e => e.MajorId);
            builder.Property(e => e.MajorId).HasMaxLength(20);
            builder.Property(e => e.MajorName).IsRequired().HasMaxLength(100);
        }
    }

    public class LecturerConfiguration : IEntityTypeConfiguration<Lecturer> {
        public void Configure(EntityTypeBuilder<Lecturer> builder) {
            builder.HasKey(e => e.LecturerId);
            builder.Property(e => e.LecturerId).HasMaxLength(20);
            builder.Property(e => e.FullName).IsRequired().HasMaxLength(100);
            builder.Property(e => e.Email).IsRequired().HasMaxLength(100);
        }
    }

    public class ClassConfiguration : IEntityTypeConfiguration<Class> {
        public void Configure(EntityTypeBuilder<Class> builder) {
            builder.HasKey(e => e.ClassId);
            builder.Property(e => e.ClassId).HasMaxLength(20);
            builder.Property(e => e.ClassName).IsRequired().HasMaxLength(100);
            builder.HasOne(d => d.Major).WithMany(p => p.Classes).HasForeignKey(d => d.MajorId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(d => d.HomeroomLecturer).WithMany(p => p.HomeroomClasses).HasForeignKey(d => d.LecturerId).OnDelete(DeleteBehavior.SetNull);
        }
    }

    public class StudentConfiguration : IEntityTypeConfiguration<Student> {
        public void Configure(EntityTypeBuilder<Student> builder) {
            builder.HasKey(e => e.StudentId);
            builder.Property(e => e.StudentId).HasMaxLength(20);
            builder.Property(e => e.FullName).IsRequired().HasMaxLength(100);
            builder.HasOne(d => d.Class).WithMany(p => p.Students).HasForeignKey(d => d.ClassId).OnDelete(DeleteBehavior.Restrict);
        }
    }

    public class SubjectConfiguration : IEntityTypeConfiguration<Subject> {
        public void Configure(EntityTypeBuilder<Subject> builder) {
            builder.HasKey(e => e.SubjectId);
            builder.Property(e => e.SubjectId).HasMaxLength(20);
            builder.Property(e => e.SubjectName).IsRequired().HasMaxLength(100);
        }
    }

    public class SemesterConfiguration : IEntityTypeConfiguration<Semester> {
        public void Configure(EntityTypeBuilder<Semester> builder) {
            builder.HasKey(e => e.SemesterId);
            builder.Property(e => e.SemesterId).HasMaxLength(20);
            builder.Property(e => e.SemesterName).IsRequired().HasMaxLength(100);
        }
    }

    public class CourseSectionConfiguration : IEntityTypeConfiguration<CourseSection> {
        public void Configure(EntityTypeBuilder<CourseSection> builder) {
            builder.HasKey(e => e.SectionId);
            builder.Property(e => e.SectionId).HasMaxLength(20);
            builder.HasOne(d => d.Subject).WithMany(p => p.CourseSections).HasForeignKey(d => d.SubjectId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(d => d.Lecturer).WithMany(p => p.CourseSections).HasForeignKey(d => d.LecturerId).OnDelete(DeleteBehavior.SetNull);
            builder.HasOne(d => d.Semester).WithMany(p => p.CourseSections).HasForeignKey(d => d.SemesterId).OnDelete(DeleteBehavior.Restrict);
        }
    }

    public class RegistrationConfiguration : IEntityTypeConfiguration<Registration> {
        public void Configure(EntityTypeBuilder<Registration> builder) {
            builder.HasKey(e => e.RegistrationId);
            builder.HasIndex(e => new { e.StudentId, e.SectionId }).IsUnique(); // BR08
            builder.HasOne(d => d.Student).WithMany(p => p.Registrations).HasForeignKey(d => d.StudentId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(d => d.CourseSection).WithMany(p => p.Registrations).HasForeignKey(d => d.SectionId).OnDelete(DeleteBehavior.Restrict);
        }
    }

    public class GradeConfiguration : IEntityTypeConfiguration<Grade> {
        public void Configure(EntityTypeBuilder<Grade> builder) {
            builder.HasKey(e => e.GradeId);
            builder.HasOne(d => d.Registration).WithOne(p => p.Grade).HasForeignKey<Grade>(d => d.RegistrationId).OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class TuitionConfiguration : IEntityTypeConfiguration<Tuition> {
        public void Configure(EntityTypeBuilder<Tuition> builder) {
            builder.HasKey(e => e.TuitionId);
            builder.Property(e => e.PricePerCredit).HasColumnType("decimal(18,2)");
            builder.Property(e => e.Amount).HasColumnType("decimal(18,2)");
            builder.Property(e => e.PaidAmount).HasColumnType("decimal(18,2)");
            builder.HasOne(d => d.Student).WithMany(p => p.Tuitions).HasForeignKey(d => d.StudentId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(d => d.Semester).WithMany(p => p.Tuitions).HasForeignKey(d => d.SemesterId).OnDelete(DeleteBehavior.Restrict);
        }
    }

    public class PaymentConfiguration : IEntityTypeConfiguration<Payment> {
        public void Configure(EntityTypeBuilder<Payment> builder) {
            builder.HasKey(e => e.PaymentId);
            builder.Property(e => e.Amount).HasColumnType("decimal(18,2)");
            builder.HasOne(d => d.Tuition).WithMany(p => p.Payments).HasForeignKey(d => d.TuitionId).OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog> {
        public void Configure(EntityTypeBuilder<AuditLog> builder) {
            builder.HasKey(e => e.LogId);
            builder.HasOne(d => d.User).WithMany().HasForeignKey(d => d.UserId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
