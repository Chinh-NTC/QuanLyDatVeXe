using Microsoft.AspNetCore.Mvc;

namespace QLDatVeXe.Areas.Management.Controllers
{
    [Area("Management")]
    public class QuanLyNhaXeController : Controller
    {
        // GET: /Management/QuanLyNhaXe
        public IActionResult Index()
        {
            return View();
        }
    }
}
