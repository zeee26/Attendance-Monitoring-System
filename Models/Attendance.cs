using EmployeeAttendanceApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZMAttendanceMonitoring.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<Employee> Employees { get; set; } // New Table Registered
        public DbSet<SystemSetting> SystemSettings { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // You can safely remove the previous modelBuilder.Ignore lines 
            // once you remove ControllerBase from the Attendance model.
        }
    }

   
    public class Attendance
    {
        public int Id { get; set; }

        // Foreign Key Linking to the Employee table
        [Required]
        [Display(Name = "Employee")]
        public int EmployeeId { get; set; }

        [ForeignKey("EmployeeId")]
        public Employee? Employee { get; set; }



        [Required]
        [Display(Name = "Date")]
        public DateOnly Date { get; set; }

        [Display(Name = "Check-In Time")]
        public TimeOnly? CheckInTime { get; set; }

        [Display(Name = "Check-Out Time")]
        public TimeOnly? CheckOutTime { get; set; }

        [Display(Name = "Total Hours Worked")]
        public string TotalHoursWorked
        {
            get
            {
                if (CheckInTime.HasValue && CheckOutTime.HasValue)
                {
                    TimeSpan elapsed = CheckOutTime.Value - CheckInTime.Value;
                    return $"{Math.Floor(elapsed.TotalHours)}h {elapsed.Minutes}m";
                }
                return "N/A";
            }
        }
    }
}
