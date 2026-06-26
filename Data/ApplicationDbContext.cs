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
    }
}