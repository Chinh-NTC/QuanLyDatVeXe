using Microsoft.AspNetCore.Mvc;

namespace QLDatVeXe.Areas.Management.Controllers
{
    [Area("Management")]
    public class QuanLyNhanVienController : Controller
    {
        // GET: /Management/QuanLyNhanVien
        public IActionResult Index()
        {
            return View();
        }
    }
}
