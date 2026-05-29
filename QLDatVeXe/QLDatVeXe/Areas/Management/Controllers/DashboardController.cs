using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLDatVeXe.Domain.Constants;
using QLDatVeXe.Domain.Enums;
using QLDatVeXe.Helpers;
using QLDatVeXe.Models;
using QLDatVeXe.Repositories.Interfaces;

namespace QLDatVeXe.Areas.management.Controllers;

[Area("management")]
public class DashboardController : Controller
{
    private readonly IDonDatVeRepository  _donRepo;
    private readonly IChuyenXeRepository  _chuyenXeRepo;
    private readonly QldatVeXeContext      _db;

    public DashboardController(IDonDatVeRepository donRepo, IChuyenXeRepository chuyenXeRepo, QldatVeXeContext db)
    {
        _donRepo      = donRepo;
        _chuyenXeRepo = chuyenXeRepo;
        _db           = db;
    }

    public async Task<IActionResult> Index()
    {
        var vaiTro = SessionHelper.GetVaiTro(HttpContext.Session);
        if (vaiTro != VaiTro.QuanLy)
            return RedirectToAction("Index", "NhanVien", new { area = "management" });

        ViewBag.User = SessionHelper.GetCurrentUser(HttpContext.Session);

        // ── Thống kê tổng quan ──────────────────────────────────────
        var tatCaDon    = await _donRepo.GetAllAsync();
        var tatCaChuyen = await _chuyenXeRepo.GetAllForAdminAsync();
        var tongKH      = await _db.Khachhang.CountAsync();
        var tongNhaXe   = await _db.Nhaxe.CountAsync();

        ViewBag.TongDon      = tatCaDon.Count;
        ViewBag.DonThanhCong = tatCaDon.Count(d => d.TrangThai == TrangThaiDon.ThanhCong);
        ViewBag.DonCho       = tatCaDon.Count(d => d.TrangThai == TrangThaiDon.ChoXuLy);
        ViewBag.DonDaHuy     = tatCaDon.Count(d => d.TrangThai == TrangThaiDon.DaHuy);
        ViewBag.ChuyenSapDi  = tatCaChuyen.Count(c => c.TrangThai == TrangThaiChuyen.SapDi);
        ViewBag.TongKH       = tongKH;
        ViewBag.TongNhaXe    = tongNhaXe;

        // Tổng doanh thu (từ đơn THANHCONG)
        var tongDT = tatCaDon
            .Where(d => d.TrangThai == TrangThaiDon.ThanhCong)
            .Sum(d => d.TongTien ?? 0);
        ViewBag.TongDoanhThu = tongDT;

        // ── Dữ liệu biểu đồ: Doanh thu 6 tháng gần nhất ────────────
        var now     = DateTime.Now;
        var thanhToan = await _db.Thanhtoan
            .Where(t => t.TrangThai == TrangThaiThanhToan.ThanhCong
                     && t.ThoiGianTt >= now.AddMonths(-5).AddDays(-now.Day + 1))
            .ToListAsync();

        var doanhThuTheoThang = Enumerable.Range(0, 6)
            .Select(i => now.AddMonths(-5 + i))
            .Select(month => new {
                Label = month.ToString("MM/yyyy"),
                Value = thanhToan
                    .Where(t => t.ThoiGianTt.HasValue
                             && t.ThoiGianTt.Value.Year  == month.Year
                             && t.ThoiGianTt.Value.Month == month.Month)
                    .Sum(t => t.SoTien)
            })
            .ToList();

        ViewBag.ChartLabels = System.Text.Json.JsonSerializer.Serialize(
            doanhThuTheoThang.Select(x => x.Label).ToList());
        ViewBag.ChartData   = System.Text.Json.JsonSerializer.Serialize(
            doanhThuTheoThang.Select(x => (long)x.Value).ToList());

        // ── Đơn hàng 8 gần nhất ─────────────────────────────────────
        ViewBag.Don8GanNhat = tatCaDon.Take(8).ToList();

        // ── Tỉ lệ trạng thái đơn (cho pie chart) ───────────────────
        ViewBag.PieLabels = System.Text.Json.JsonSerializer.Serialize(
            new[] { "Thành công", "Chờ xử lý", "Đã hủy" });
        ViewBag.PieData   = System.Text.Json.JsonSerializer.Serialize(
            new[] { ViewBag.DonThanhCong, ViewBag.DonCho, ViewBag.DonDaHuy });

        return View();
    }
}
