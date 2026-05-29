using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLDatVeXe.Models;

namespace QLDatVeXe.Areas.Management.Controllers
{
    [Area("Management")]
    public class QuanLyDonDatVeController : Controller
    {
        private readonly QldatVeXeContext _context;

        public QuanLyDonDatVeController(QldatVeXeContext context)
        {
            _context = context;
        }

        // GET: /Management/QuanLyDonDatVe
        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "Quản lý Đơn đặt vé";
            ViewData["AdminNav"] = "donhang";

            var Dondatve = await _context.Dondatve
                .Include(d => d.MaKhNavigation)
                .OrderByDescending(d => d.NgayDat)
                .ToListAsync();

            ViewBag.Dondatve = Dondatve;

            return View("~/Areas/Management/Views/QuanLyDonDatVe/Index.cshtml");
        }
    }
}
