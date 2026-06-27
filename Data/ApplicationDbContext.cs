using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Student_Portal.Models;

namespace Student_Portal.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<StudentForm> StudentForms { get; set; }
    public DbSet<Document> Documents { get; set; }
    public DbSet<Course> Courses { get; set; }
    public DbSet<Result> Results { get; set; }
    public DbSet<GpaRecord> GpaRecords { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }
    public DbSet<CourseRegistration> CourseRegistrations { get; set; }
    public DbSet<RegistrationSubmission> RegistrationSubmissions { get; set; }
    public DbSet<LecturerDepartment> LecturerDepartments { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<StudentForm>()
            .HasOne(sf => sf.User)
            .WithMany()
            .HasForeignKey(sf => sf.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Document>()
            .HasOne(d => d.User)
            .WithMany()
            .HasForeignKey(d => d.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Course>()
            .HasIndex(c => new { c.CourseCode, c.Session, c.Semester })
            .IsUnique();

        builder.Entity<Result>()
            .HasOne(r => r.User)
            .WithMany()
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Result>()
            .HasOne(r => r.Course)
            .WithMany()
            .HasForeignKey(r => r.CourseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Result>()
            .HasIndex(r => new { r.UserId, r.CourseId })
            .IsUnique();

        builder.Entity<GpaRecord>()
            .HasOne(g => g.User)
            .WithMany()
            .HasForeignKey(g => g.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<GpaRecord>()
            .HasIndex(g => new { g.UserId, g.Session, g.Semester })
            .IsUnique();

        builder.Entity<GpaRecord>()
            .Property(g => g.GPA)
            .HasColumnType("decimal(4,2)");

        builder.Entity<GpaRecord>()
            .Property(g => g.CGPA)
            .HasColumnType("decimal(4,2)");

        builder.Entity<AuditLog>()
            .HasOne(a => a.User)
            .WithMany()
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<CourseRegistration>()
            .HasOne(cr => cr.User)
            .WithMany()
            .HasForeignKey(cr => cr.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<CourseRegistration>()
            .HasOne(cr => cr.Course)
            .WithMany()
            .HasForeignKey(cr => cr.CourseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<CourseRegistration>()
            .HasIndex(cr => new { cr.UserId, cr.CourseId })
            .IsUnique();

        builder.Entity<RegistrationSubmission>()
            .HasOne(rs => rs.User)
            .WithMany()
            .HasForeignKey(rs => rs.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<RegistrationSubmission>()
            .HasIndex(rs => new { rs.UserId, rs.Session, rs.Semester })
            .IsUnique();

        builder.Entity<LecturerDepartment>()
            .HasOne(ld => ld.User)
            .WithMany()
            .HasForeignKey(ld => ld.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<LecturerDepartment>()
            .HasIndex(ld => new { ld.UserId, ld.Department })
            .IsUnique();
    }
}