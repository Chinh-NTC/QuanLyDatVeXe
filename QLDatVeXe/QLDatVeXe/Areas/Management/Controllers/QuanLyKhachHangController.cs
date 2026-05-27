using Microsoft.AspNetCore.Mvc;

namespace QLDatVeXe.Areas.Management.Controllers
{
    [Area("Management")]
    public class QuanLyKhachHangController : Controller
    {
        // GET: /Management/QuanLyKhachHang
        public IActionResult Index()
        {
            return View();
        }
    }
}
