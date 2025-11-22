using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using LeaveManagementSystem.Models;
using LeaveManagementSystem.DataAccessLayer;
using LeaveManagementSystem.Helpers;

namespace LeaveManagementSystem.Controllers
{
    public class LeaveController : Controller
    {
        private readonly LeaveDAL _leaveDAL;
        private readonly EmailService _emailService;
        private readonly EmployeeDAL _employeeDAL;
        private readonly IConfiguration _config;

        public LeaveController(IConfiguration config, EmailService emailService)
        {
            _leaveDAL = new LeaveDAL(config);
            _emailService = emailService;
            _employeeDAL = new EmployeeDAL(config);
            _config = config;
        }

         /*public LeaveController(IConfiguration config)
        {
            _leaveDAL = new LeaveDAL(config);
        }
         */
        public IActionResult Dashboard()
        {
            if (!IsEmployee()) return RedirectToLogin();

            ViewBag.UserName = HttpContext.Session.GetString("UserName") ?? "Employee";
            return View();
        }

        public IActionResult RequestLeave()
        {
            if (!IsEmployee()) return RedirectToLogin();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RequestLeave(LeaveRequest leave)
        {
            if (!IsEmployee()) return RedirectToLogin();

            var userIdStr = HttpContext.Session.GetString("UserId");
            if (!int.TryParse(userIdStr, out int userId))
            {
                return RedirectToLogin();
            }

            leave.EmployeeId = userId;
            int leaveId = _leaveDAL.SubmitLeave(leave);

            bool emailSent = false;

            // Get employee details
            var employee = _employeeDAL.GetEmployeeById(userId);
            if (employee != null)
            {
                // Get admin notification email from configuration
                var adminNotificationEmail = _config.GetSection("EmailSettings")["SenderEmail"] ?? "leavemanagementsystem001@gmail.com";

                var subject = $"New Leave Request Submitted - #{leaveId}";
                var body = $@"
<div style='font-family:Segoe UI; padding:20px;'>
    <h2 style='color:#2c3e50;'>New Leave Request Notification</h2>
    <p>Dear Admin,</p>
    <p>A new leave request has been submitted by <strong>{employee.Name}</strong>.</p>
    <div style='background-color:#f8f9fa; padding:15px; border-radius:5px; margin:15px 0;'>
        <p><strong>Request ID:</strong> #{leaveId}</p>
        <p><strong>Employee Name:</strong> {employee.Name}</p>
        <p><strong>Employee Email:</strong> {employee.Email}</p>
        <p><strong>Leave Type:</strong> {leave.LeaveType}</p>
        <p><strong>Start Date:</strong> {leave.StartDate:dd MMM yyyy}</p>
        <p><strong>End Date:</strong> {leave.EndDate:dd MMM yyyy}</p>
        <p><strong>Reason:</strong> {leave.Reason}</p>
        <p><strong>Status:</strong> <span style='color:orange; font-weight:bold;'>Pending</span></p>
    </div>
    <p>Please review and take appropriate action on this leave request.</p>
    <hr/>
    <p style='font-size:12px; color:#888;'>This is an automated message from Leave Management System.</p>
</div>";

                // Send email notification to admin email
                try
                {
                    _emailService.SendEmail(adminNotificationEmail, subject, body);
                    emailSent = true;
                }
                catch (Exception)
                {
                    // Log error but don't fail the request submission
                    // You might want to add proper logging here
                    emailSent = false;
                }
            }

            // Set message based on whether email was actually sent
            if (emailSent)
            {
                TempData["Message"] = "Leave request submitted successfully. Admin has been notified.";
            }
            else
            {
                TempData["Message"] = "Leave request submitted successfully. (Could not send notification email - employee information not found or email service unavailable)";
            }
            return RedirectToAction("Dashboard");
        }

        public IActionResult LeaveHistory()
        {
            if (!IsEmployee()) return RedirectToLogin();

            var userIdStr = HttpContext.Session.GetString("UserId");
            if (!int.TryParse(userIdStr, out int userId))
            {
                return RedirectToLogin();
            }

            var history = _leaveDAL.GetLeaveHistoryByEmployee(userId);
            return View(history);
        }

        public IActionResult AdminDashboard(string status, string keyword)
        {
            if (!IsAdmin()) return RedirectToLogin();

            var filteredRequests = _leaveDAL.GetFilteredLeaveRequests(status, keyword);
            var summary = _leaveDAL.GetLeaveSummary();
            var monthlyData = _leaveDAL.GetMonthlyLeaveCounts();

            ViewBag.Summary = summary;
            ViewBag.MonthlyData = monthlyData;

            return View(filteredRequests);
        }

        [HttpPost]
        public IActionResult UpdateStatus(int id, string status)
        {
            if (!IsAdmin()) return RedirectToLogin();

            // Normalize status to ensure it's never null or empty
            var normalizedStatus = string.IsNullOrEmpty(status) ? "Pending" : status;
            
            _leaveDAL.UpdateLeaveStatus(id, normalizedStatus);

            // 🔔 Fetch employee email (replace with actual DB logic)
            var leaveRequest = _leaveDAL.GetLeaveRequestById(id); // You need to implement this method
            
            // Check if leaveRequest is null before accessing its properties
            if (leaveRequest != null)
            {
                var employeeEmail = leaveRequest.EmployeeEmail ?? "employee@example.com"; // Replace with actual field

                var subject = $"Leave Request #{id} - {normalizedStatus}";
                var body = $@"
<div style='font-family:Segoe UI; padding:20px;'>
    <h2 style='color:#2c3e50;'>Leave Request Update</h2>
    <p>Dear <strong>{leaveRequest.EmployeeName}</strong>,</p>
    <p>Your leave request <strong>(ID: {id})</strong> has been <span style='color:green;'>{normalizedStatus}</span>.</p>
    <p><strong>Leave Type:</strong> {leaveRequest.LeaveType}<br/>
       <strong>Dates:</strong> {leaveRequest.StartDate:dd MMM yyyy} to {leaveRequest.EndDate:dd MMM yyyy}<br/>
       <strong>Reason:</strong> {leaveRequest.Reason}</p>
    <hr/>
    <p style='font-size:12px; color:#888;'>This is an automated message from Leave Management System.</p>
</div>";

                _emailService.SendEmail(employeeEmail, subject, body);
                TempData["Message"] = $"Leave request #{id} marked as {normalizedStatus}. Email sent to employee.";
            }
            else
            {
                TempData["Message"] = $"Leave request #{id} marked as {normalizedStatus}. (Could not send email - leave request not found)";
            }
            return RedirectToAction("AdminDashboard");
        }

        public IActionResult ExportToExcel()
        {
            if (!IsAdmin()) return RedirectToLogin();

            var status = HttpContext.Request.Query["status"].ToString() ?? string.Empty;
            var keyword = HttpContext.Request.Query["keyword"].ToString() ?? string.Empty;

            var data = _leaveDAL.GetFilteredLeaveRequests(status, keyword);
            var stream = ExcelExporter.GenerateLeaveExcel(data);

            return File(stream.ToArray(),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "LeaveRequests.xlsx");
        }

        // 🔒 Role helpers
        private bool IsAdmin() => HttpContext.Session.GetString("Role") == "Admin";
        private bool IsEmployee() => HttpContext.Session.GetString("Role") == "Employee";
        private IActionResult RedirectToLogin() => RedirectToAction("Login", "Account");
    }
}
