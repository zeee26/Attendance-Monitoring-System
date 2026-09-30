using System.ComponentModel.DataAnnotations;

namespace EmployeeAttendanceApp.Models
{
    public class Employee
    {
        public int Id { get; set; }

        [Required]
        [RegularExpression(@"^[A-Za-z ]+$", ErrorMessage = "Only letters are allowed.")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^[A-Za-z ]+$", ErrorMessage = "Only letters are allowed.")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        // Helper
        public string FullName => $"{FirstName} {LastName}";
    }
}
