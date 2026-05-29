using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLDatVeXe.Domain.Enums;
using QLDatVeXe.Helpers;
using QLDatVeXe.Models;

namespace QLDatVeXe.Areas.management.Controllers;

[Area("management")]
public class QuanLyKhachHangController : Controller
{
    private readonly QldatVeXeContext _db;

    public QuanLyKhachHangController(QldatVeXeContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index(string? search, string? trangThai, int page = 1)
    {
        var vaiTro = SessionHelper.GetVaiTro(HttpContext.Session);
        if (vaiTro != VaiTro.QuanLy)
            return RedirectToAction("Index", "NhanVien", new { area = "management" });

        ViewBag.PageName  = "KhachHang";
        ViewBag.Search     = search;
        ViewBag.TrangThai  = trangThai;

        var query = _db.Khachhang
            .Include(k => k.Dondatve)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(k => k.HoTen.Contains(search)
                                  || k.Sdt.Contains(search)
                                  || k.TenDangNhap.Contains(search));

        if (trangThai == "active")   query = query.Where(k => k.Trangthai);
        if (trangThai == "inactive") query = query.Where(k => !k.Trangthai);

        int pageSize  = 10;
        int total     = await query.CountAsync();
        var khachHangs = await query
            .OrderByDescending(k => k.NgayTao)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        ViewBag.KhachHangs = khachHangs;
        ViewBag.Page       = page;
        ViewBag.TotalPages = (int)Math.Ceiling(total / (double)pageSize);
        ViewBag.Total      = total;

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> DoiTrangThai(string maKH)
    {
        var vaiTro = SessionHelper.GetVaiTro(HttpContext.Session);
        if (vaiTro != VaiTro.QuanLy)
            return Forbid();

        var kh = await _db.Khachhang.FindAsync(maKH);
        if (kh is null) return NotFound();

        kh.Trangthai = !kh.Trangthai;
        await _db.SaveChangesAsync();

        TempData["Success"] = kh.Trangthai
            ? $"Đã mở khóa tài khoản {kh.HoTen}."
            : $"Đã khóa tài khoản {kh.HoTen}.";

        return RedirectToAction(nameof(Index));
    }
}
