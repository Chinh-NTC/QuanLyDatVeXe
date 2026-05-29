using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLDatVeXe.Domain.Constants;
using QLDatVeXe.Models;

namespace QLDatVeXe.Areas.Management.Controllers
{
    [Area("Management")]
    public class BaoCaoController : Controller
    {
        private readonly QldatVeXeContext _context;

        public BaoCaoController(QldatVeXeContext context)
        {
            _context = context;
        }

        // GET: /Management/BaoCao
        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "Báo cáo doanh thu";
            ViewData["AdminNav"] = "baocao";

            // Tổng doanh thu (các đơn đã thanh toán / hoàn thành)
            decimal totalRevenue = await _context.Dondatve
                .Where(d => d.TrangThai == TrangThaiDon.ThanhCong)
                .SumAsync(d => d.TongTien ?? 0);

            // Tổng vé bán
            int totalTickets = await _context.Chitietdatve
                .Include(ct => ct.MaDonNavigation)
                .Where(ct => ct.MaDonNavigation.TrangThai != TrangThaiDon.DaHuy)
                .CountAsync();

            // Chuyến hoàn thành
            int completedTrips = await _context.Chuyenxe
                .Where(c => c.TrangThai == TrangThaiChuyen.HoanThanh)
                .CountAsync();

            // Doanh thu theo nhà xe
            var revenueByNhaXe = await _context.Dondatve
                .Where(d => d.TrangThai == TrangThaiDon.ThanhCong)
                .Join(_context.Chitietdatve, d => d.MaDon, ct => ct.MaDon, (d, ct) => new { d, ct })
                .Join(_context.Chuyenxe, x => x.ct.MaChuyen, c => c.MaChuyen, (x, c) => new { x.d, x.ct, c })
                .Join(_context.Xe, x => x.c.BienSo, xe => xe.BienSo, (x, xe) => new { x.d, x.ct, x.c, xe })
                .Join(_context.Nhaxe, x => x.xe.MaNhaXe, nx => nx.MaNhaXe, (x, nx) => new { x.d, x.ct, nx })
                .GroupBy(x => new { x.nx.MaNhaXe, x.nx.TenNhaXe })
                .Select(g => new
                {
                    MaNhaXe = g.Key.MaNhaXe,
                    TenNhaXe = g.Key.TenNhaXe,
                    SoVeBan = g.Count(),
                    DoanhThu = g.Sum(x => x.ct.GiaVeLucDat)
                })
                .OrderByDescending(r => r.DoanhThu)
                .ToListAsync();

            ViewBag.TotalRevenue = totalRevenue;
            ViewBag.TotalTickets = totalTickets;
            ViewBag.CompletedTrips = completedTrips;
            
            // Just average rating mockup or calculation (if needed)
            var avgRating = await _context.Danhgia.AverageAsync(d => d.DiemDanhGia) ?? 0;
            ViewBag.AvgRating = avgRating;

            // Truyền dữ liệu report
            ViewBag.RevenueByNhaXe = revenueByNhaXe.Select(r => new {
                TenNhaXe = r.TenNhaXe,
                SoVeBan = r.SoVeBan,
                DoanhThu = r.DoanhThu,
                TyLe = totalRevenue > 0 ? (r.DoanhThu / totalRevenue) * 100 : 0
            }).ToList();

            return View("~/Areas/Management/Views/BaoCao/Index.cshtml");
        }
    }
}
