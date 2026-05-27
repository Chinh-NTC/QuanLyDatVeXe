using Microsoft.AspNetCore.Mvc;

namespace QLDatVeXe.Areas.Management.Controllers
{
    [Area("Management")]
    public class BaoCaoController : Controller
    {
        // GET: /Management/BaoCao
        public IActionResult Index()
        {
            return View();
        }
    }
}
