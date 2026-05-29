using Microsoft.AspNetCore.Mvc;
using QLDatVeXe.Models;
using QLDatVeXe.Repositories.Interfaces;

namespace QLDatVeXe.Controllers
{
    public class NhaXeController : Controller
    {
        private readonly INhaxeRepository _nhaxeRepo;

        public NhaXeController(INhaxeRepository nhaxeRepo)
        {
            _nhaxeRepo = nhaxeRepo;
        }

        // GET: /NhaXe
        [HttpGet]
        [Route("NhaXe")]
        public async Task<IActionResult> Index()
        {
            var Nhaxe = await _nhaxeRepo.GetAllAsync();
            ViewBag.Nhaxe = Nhaxe;
            return View("~/Views/NhaXe/Index.cshtml");
        }

        // GET: /NhaXe/Detail
        [HttpGet]
        [Route("NhaXe/Detail")]
        public async Task<IActionResult> Detail(string ma)
        {
            if (string.IsNullOrEmpty(ma)) return RedirectToAction("Index");

            var nhaXe = await _nhaxeRepo.GetNhaXeWithXeAsync(ma);
            if (nhaXe == null) return NotFound();

            ViewBag.NhaXe = nhaXe;

            var Danhgia = await _nhaxeRepo.GetDanhGiaByNhaXeAsync(ma);
            ViewBag.Danhgia = Danhgia;

            return View("~/Views/NhaXe/Detail.cshtml");
        }

        // GET: /NhaXe/Xe
        [HttpGet]
        [Route("NhaXe/Xe")]
        public async Task<IActionResult> Xe(string bienSo)
        {
            if (string.IsNullOrEmpty(bienSo)) return RedirectToAction("Index");

            var xe = await _nhaxeRepo.GetXeWithDanhGiaAsync(bienSo);
            if (xe == null) return NotFound();

            ViewBag.Xe = xe;
            return View("~/Views/NhaXe/Xe.cshtml");
        }
    }
}
