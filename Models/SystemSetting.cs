using System.ComponentModel.DataAnnotations;

namespace EmployeeAttendanceApp.Models
{
    public class SystemSetting
    {
        [Key]
        public string SettingKey { get; set; } = string.Empty;

        [Required]
        public string SettingValue { get; set; } = string.Empty;
    }
}
