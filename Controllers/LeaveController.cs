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

        public LeaveController(IConfiguration config, EmailService emailService)
        {
            _leaveDAL = new LeaveDAL(config);
            _emailService = emailService;
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
            _leaveDAL.SubmitLeave(leave);

            TempData["Message"] = "Leave request submitted successfully.";
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

            _leaveDAL.UpdateLeaveStatus(id, status ?? "Pending");

            // 🔔 Fetch employee email (replace with actual DB logic)
            var leaveRequest = _leaveDAL.GetLeaveRequestById(id); // You need to implement this method
            var employeeEmail = leaveRequest?.EmployeeEmail ?? "employee@example.com"; // Replace with actual field

            var subject = $"Leave Request #{id} - {status}";
            var body = $@"
<div style='font-family:Segoe UI; padding:20px;'>
    <h2 style='color:#2c3e50;'>Leave Request Update</h2>
    <p>Dear <strong>{leaveRequest.EmployeeName}</strong>,</p>
    <p>Your leave request <strong>(ID: {id})</strong> has been <span style='color:green;'>{status}</span>.</p>
    <p><strong>Leave Type:</strong> {leaveRequest.LeaveType}<br/>
       <strong>Dates:</strong> {leaveRequest.StartDate:dd MMM yyyy} to {leaveRequest.EndDate:dd MMM yyyy}<br/>
       <strong>Reason:</strong> {leaveRequest.Reason}</p>
    <hr/>
    <p style='font-size:12px; color:#888;'>This is an automated message from Leave Management System.</p>
</div>";


            _emailService.SendEmail(employeeEmail, subject, body);

            TempData["Message"] = $"Leave request #{id} marked as {status}. Email sent to employee.";
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
