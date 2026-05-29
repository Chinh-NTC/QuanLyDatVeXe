using Microsoft.AspNetCore.Mvc;
using QLDatVeXe.Domain.Enums;
using QLDatVeXe.Helpers;
using QLDatVeXe.Models;
using QLDatVeXe.Repositories.Interfaces;

namespace QLDatVeXe.Areas.management.Controllers;

[Area("management")]
public class QuanLyChuyenXeController : Controller
{
    private readonly IChuyenXeRepository _chuyenXeRepo;

    public QuanLyChuyenXeController(IChuyenXeRepository chuyenXeRepo)
        => _chuyenXeRepo = chuyenXeRepo;

    private bool IsQuanLy() => SessionHelper.GetVaiTro(HttpContext.Session) == VaiTro.QuanLy;
    private bool IsStaff()  => SessionHelper.GetVaiTro(HttpContext.Session) is VaiTro.QuanLy or VaiTro.NhanVien;

    public async Task<IActionResult> Index()
    {
        if (!IsStaff()) return RedirectToAction("Login", "TaiKhoan", new { area = "" });
        ViewBag.User    = SessionHelper.GetCurrentUser(HttpContext.Session);
        ViewBag.DsTuyen = await _chuyenXeRepo.GetAllTuyenAsync();
        ViewBag.DsXe    = await _chuyenXeRepo.GetAllXeAsync();
        var dsChuyen    = await _chuyenXeRepo.GetAllForAdminAsync();
        return View(dsChuyen);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Them(Chuyenxe chuyen)
    {
        if (!IsQuanLy()) return Forbid();
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Dữ liệu không hợp lệ.";
            return RedirectToAction("Index");
        }
        await _chuyenXeRepo.ThemChuyenAsync(chuyen);
        TempData["Success"] = "Thêm chuyến xe thành công!";
        return RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Sua(Chuyenxe chuyen)
    {
        if (!IsQuanLy()) return Forbid();
        var existing = await _chuyenXeRepo.GetByIdAsync(chuyen.MaChuyen);
        if (existing is null) return NotFound();

        existing.MaTuyen  = chuyen.MaTuyen;
        existing.BienSo   = chuyen.BienSo;
        existing.NgayDi   = chuyen.NgayDi;
        existing.GioDi    = chuyen.GioDi;
        existing.GiaVe    = chuyen.GiaVe;
        existing.TrangThai= chuyen.TrangThai;
        await _chuyenXeRepo.SuaChuyenAsync(existing);
        TempData["Success"] = "Cập nhật chuyến xe thành công!";
        return RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Huy(string maChuyen)
    {
        if (!IsQuanLy()) return Forbid();
        await _chuyenXeRepo.HuyChuyenAsync(maChuyen);
        TempData["Success"] = "Đã hủy chuyến xe.";
        return RedirectToAction("Index");
    }
}
