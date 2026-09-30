using EmployeeAttendanceApp.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
//using ZMAttendanceMonitoring.Data;
using ZMAttendanceMonitoring.Models;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// 1. Try to read from appsettings.json or Azure standard injection
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// 2. If it's null or completely empty, fall back directly to the Environment Variable
if (string.IsNullOrEmpty(connectionString))
{
    connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection") 
                       ?? Environment.GetEnvironmentVariable("SQLCONNSTR_DefaultConnection");
}

// Register DbContext with the resolved string
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));


// Add Cookie Authentication services
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Attendance/Login"; // Redirect target if unauthorized
        options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (!app.Environment.IsDevelopment())
//{
//    //app.UseExceptionHandler("/Home/Error");
//    //// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
//    app.UseHsts();

//}

// Seed Default Password if it doesn't exist
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Database.EnsureCreated();
    if (!context.SystemSettings.Any(s => s.SettingKey == "AdminPasswordHash"))
    {
        // Default password: Admin123 (hashed using BCrypt)
        var defaultHash = BCrypt.Net.BCrypt.HashPassword("Admin123");
        context.SystemSettings.Add(new SystemSetting { SettingKey = "AdminPasswordHash", SettingValue = defaultHash });
        context.SaveChanges();
    }
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Default
//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Attendance}/{action=Index}/{id?}");
app.MapControllerRoute(
    name: "home_explicit",
    pattern: "Home",
    defaults: new { controller = "Attendance", action = "Index" });

// Admin
app.MapControllerRoute(
    name: "admin",
    pattern: "Admin",
    defaults: new { controller = "Attendance", action = "History" });

// Error
app.MapControllerRoute(
    name: "error",
    pattern: "Error",
    //defaults: new { controller = "Home", action = "Error" });
    defaults: new { controller = "Attendance", action = "Error" });

// Privacy
app.MapControllerRoute(
    name: "privacy_policy",
    pattern: "PrivacyPolicy",
    //defaults: new { controller = "Home", action = "Error" });
    defaults: new { controller = "Attendance", action = "PrivacyPolicy" });

// Global default
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Attendance}/{action=Index}/{id?}");

app.Run();
