using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLDatVeXe.Domain.Enums;
using QLDatVeXe.Helpers;
using QLDatVeXe.Models;

namespace QLDatVeXe.Areas.management.Controllers;

[Area("management")]
public class QuanLyNhaXeController : Controller
{
    private readonly QldatVeXeContext _db;

    public QuanLyNhaXeController(QldatVeXeContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index(string? search)
    {
        var vaiTro = SessionHelper.GetVaiTro(HttpContext.Session);
        if (vaiTro != VaiTro.QuanLy)
            return RedirectToAction("Index", "NhanVien", new { area = "management" });

        ViewBag.PageName = "NhaXe";
        ViewBag.Search   = search;

        var query = _db.Nhaxe
            .Include(n => n.Xe)
                .ThenInclude(x => x.Chuyenxe)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(n => n.TenNhaXe.Contains(search)
                                  || (n.Sdt != null && n.Sdt.Contains(search)));

        var nhaXes = await query.ToListAsync();
        ViewBag.NhaXes = nhaXes;

        return View();
    }

    public async Task<IActionResult> ChiTiet(string maNhaXe)
    {
        var vaiTro = SessionHelper.GetVaiTro(HttpContext.Session);
        if (vaiTro != VaiTro.QuanLy)
            return RedirectToAction("Index", "NhanVien", new { area = "management" });

        var nhaXe = await _db.Nhaxe
            .Include(n => n.Xe)
                .ThenInclude(x => x.Ghe)
            .Include(n => n.Xe)
                .ThenInclude(x => x.Chuyenxe)
            .FirstOrDefaultAsync(n => n.MaNhaXe == maNhaXe);

        if (nhaXe is null) return NotFound();

        // Đánh giá các xe của nhà xe
        var maChuyenList = nhaXe.Xe
            .SelectMany(x => x.Chuyenxe)
            .Select(c => c.MaChuyen)
            .ToList();

        var danhGias = await _db.Danhgia
            .Where(d => maChuyenList.Contains(d.MaChuyen))
            .Include(d => d.MaKhNavigation)
            .OrderByDescending(d => d.NgayDanhGia)
            .Take(10)
            .ToListAsync();

        ViewBag.NhaXe    = nhaXe;
        ViewBag.DanhGias = danhGias;

        double avgDiem = danhGias.Any() ? danhGias.Average(d => d.DiemDanhGia ?? 0) : 0;
        ViewBag.DiemTB  = avgDiem;
        ViewBag.PageName = "NhaXe";

        return View();
    }
}
