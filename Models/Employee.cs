namespace LeaveManagementSystem.Models
{
    public class Employee
    {
        public int Id { get; set; }               // Primary Key
        public string? Name { get; set; }         // Full name
        public string? Email { get; set; }        // Used for login
        public string? Password { get; set; }     // Plain or hashed password
        public string? Role { get; set; }         // "Admin" or "Employee"
    }
}
