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

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        builder.Entity<StudentForm>()
            .HasOne(sf => sf.User)
            .WithMany()
            .HasForeignKey(sf => sf.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}