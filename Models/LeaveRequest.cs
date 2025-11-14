namespace LeaveManagementSystem.Models
{
    public class LeaveRequest
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public string? LeaveType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Reason { get; set; }
        public string? Status { get; set; }

        public string EmployeeEmail { get; set; } = string.Empty;

        public string EmployeeName { get; set; } = string.Empty;


    }
}
