using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Infrastructure;
using Student_Portal.Data;
using Student_Portal.Models;
using Student_Portal.Repositories;
using Student_Portal.Services;

QuestPDF.Settings.License = LicenseType.Community;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Use in-memory data protection keys so auth cookies are invalidated whenever the app restarts,
// instead of silently resuming a previous login session.
builder.Services.AddDataProtection()
    .UseEphemeralDataProtectionProvider();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedEmail = true;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;

    // Login attempt monitoring: lock an account out temporarily after repeated failed attempts.
    options.Lockout.AllowedForNewUsers = true;
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    // Session timeout: auto sign-out after 10 minutes of inactivity.
    options.ExpireTimeSpan = TimeSpan.FromMinutes(10);
    options.SlidingExpiration = true;
    options.AccessDeniedPath = "/Home/AccessDenied";
});

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<IDocumentRepository, DocumentRepository>();
builder.Services.AddScoped<ICourseRepository, CourseRepository>();
builder.Services.AddScoped<IResultRepository, ResultRepository>();
builder.Services.AddScoped<IGpaRecordRepository, GpaRecordRepository>();
builder.Services.AddScoped<IAuditLogRepository, AuditLogRepository>();
builder.Services.AddScoped<ICourseRegistrationRepository, CourseRegistrationRepository>();
builder.Services.AddScoped<IRegistrationSubmissionRepository, RegistrationSubmissionRepository>();
builder.Services.AddScoped<ILecturerDepartmentRepository, LecturerDepartmentRepository>();

builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IDocumentService, DocumentService>();
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<IResultService, ResultService>();
builder.Services.AddScoped<IGpaService, GpaService>();
builder.Services.AddScoped<IRegistrationSlipService, RegistrationSlipService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IAuditService, AuditService>();
builder.Services.AddScoped<ICourseRegistrationService, CourseRegistrationService>();
builder.Services.AddScoped<ILecturerService, LecturerService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Seed initial roles and super admin
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    // Create roles
    string[] roleNames = { "SuperAdmin", "Admin", "ExamOfficer", "Lecturer", "Student" };
    foreach (var roleName in roleNames)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }

    // Create super admin
    var superAdminEmail = "superadmin@portal.com";
    if (await userManager.FindByEmailAsync(superAdminEmail) == null)
    {
        var superAdmin = new ApplicationUser
        {
            UserName = superAdminEmail,
            Email = superAdminEmail,
            EmailConfirmed = true,
            FirstName = "Super",
            LastName = "Admin",
            IsActive = true
        };

        var result = await userManager.CreateAsync(superAdmin, "SuperAdmin123!");
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(superAdmin, "SuperAdmin");
        }
    }

    // ExamOfficer is no longer a standalone role - it's only granted as an add-on to Lecturer.
    // Backfill any account that ended up with ExamOfficer but not Lecturer (idempotent, safe to run every startup).
    foreach (var user in await userManager.GetUsersInRoleAsync("ExamOfficer"))
    {
        if (!await userManager.IsInRoleAsync(user, "Lecturer"))
        {
            await userManager.AddToRoleAsync(user, "Lecturer");
        }
    }
}

app.Run();