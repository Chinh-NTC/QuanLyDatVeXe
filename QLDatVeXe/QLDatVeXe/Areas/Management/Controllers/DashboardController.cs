using Microsoft.AspNetCore.Mvc;

namespace QLDatVeXe.Areas.Management.Controllers
{
    [Area("Management")]
    public class DashboardController : Controller
    {
        // GET: /Management/Dashboard
        public IActionResult Index()
        {
            ViewData["Title"] = "Dashboard";
            ViewData["AdminNav"] = "dashboard";
            return View();
        }
    }
}
