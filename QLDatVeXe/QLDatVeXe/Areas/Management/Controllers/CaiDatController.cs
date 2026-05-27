using Microsoft.AspNetCore.Mvc;

namespace QLDatVeXe.Areas.Management.Controllers
{
    [Area("Management")]
    public class CaiDatController : Controller
    {
        // GET: /Management/CaiDat
        public IActionResult Index()
        {
            return View();
        }
    }
}
