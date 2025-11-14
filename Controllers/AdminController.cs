using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace LeaveManagementSystem.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Dashboard()
        {
            if (HttpContext.Session.GetString("Role") != "Admin")
                return RedirectToAction("Login", "Account");

            ViewBag.UserName = HttpContext.Session.GetString("UserName");
            return View();
        }
    }
}
