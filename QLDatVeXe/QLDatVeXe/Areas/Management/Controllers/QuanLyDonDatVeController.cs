using Microsoft.AspNetCore.Mvc;

namespace QLDatVeXe.Areas.Management.Controllers
{
    [Area("Management")]
    public class QuanLyDonDatVeController : Controller
    {
        // GET: /Management/QuanLyDonDatVe
        public IActionResult Index()
        {
            return View("~/Areas/Management/Views/QuanLyDonDatVe/Index.cshtml");
        }
    }
}
