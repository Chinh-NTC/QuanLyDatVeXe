using Microsoft.AspNetCore.Mvc;

namespace QLDatVeXe.Areas.Management.Controllers
{
    [Area("Management")]
    public class QuanLyChuyenXeController : Controller
    {
        // GET: /Management/QuanLyChuyenXe
        public IActionResult Index()
        {
            ViewData["Title"] = "Quản lý Chuyến xe";
            ViewData["AdminNav"] = "chuyenxe";
            return View();
        }
    }
}
