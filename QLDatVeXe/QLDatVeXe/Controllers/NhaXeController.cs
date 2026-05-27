using Microsoft.AspNetCore.Mvc;

namespace QLDatVeXe.Controllers
{
    public class NhaXeController : Controller
    {
        // GET: /NhaXe
        [HttpGet]
        [Route("NhaXe")]
        public IActionResult Index()
        {
            return View("~/Views/NhaXe/Index.cshtml");
        }

        // GET: /NhaXe/Detail
        [HttpGet]
        [Route("NhaXe/Detail")]
        public IActionResult Detail(string? ma)
        {
            return View("~/Views/NhaXe/Detail.cshtml");
        }
    }
}
