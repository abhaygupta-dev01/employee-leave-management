using Microsoft.AspNetCore.Mvc;
using LeaveManagementSystem.Models;
using LeaveManagementSystem.DataAccessLayer;
//using Microsoft.AspNetCore.Http;


namespace LeaveManagementSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly EmployeeDAL _dal;

        public AccountController(IConfiguration config)
        {
            _dal = new EmployeeDAL(config);
        }

        public IActionResult Login() => View();

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            var user = _dal.Login(email, password);
            if (user != null)
            {
                HttpContext.Session.SetString("UserId", user.Id.ToString());
                HttpContext.Session.SetString("Role", user.Role);
                HttpContext.Session.SetString("UserName", user.Name);

                if (user.Role == "Admin")
                    return RedirectToAction("Dashboard", "Admin");
                else
                    return RedirectToAction("Dashboard", "Leave");
            }

            ViewBag.Error = "Invalid email or password.";
            return View();
        }

        public IActionResult Register() => View();

        [HttpPost]
        public IActionResult Register(Employee emp)
        {
            _dal.Register(emp);
            return RedirectToAction("Login");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
