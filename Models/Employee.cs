using System.ComponentModel.DataAnnotations;

namespace EmployeeAttendanceApp.Models
{
    public class Employee
    {
        public int Id { get; set; }

        [Required]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Only letters are allowed.")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Only letters are allowed.")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        // Helper property for easier display in dropdowns
        public string FullName => $"{FirstName} {LastName}";
    }
}
