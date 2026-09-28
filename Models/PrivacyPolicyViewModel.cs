namespace ZMAttendanceMonitoring.Models
{
    public class PrivacyPolicyViewModel
    {
        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
