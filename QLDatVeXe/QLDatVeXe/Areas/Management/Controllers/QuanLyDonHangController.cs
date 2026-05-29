using Microsoft.AspNetCore.Mvc;
using QLDatVeXe.Domain.Enums;
using QLDatVeXe.Helpers;
using QLDatVeXe.Repositories.Interfaces;

namespace QLDatVeXe.Areas.management.Controllers;

[Area("management")]
public class QuanLyDonHangController : Controller
{
    private readonly IDonDatVeRepository _donRepo;

    public QuanLyDonHangController(IDonDatVeRepository donRepo) => _donRepo = donRepo;

    private bool IsQuanLy() => SessionHelper.GetVaiTro(HttpContext.Session) == VaiTro.QuanLy;

    public async Task<IActionResult> Index(string? trangThai = null)
    {
        if (!IsQuanLy())
            return RedirectToAction("Login", "TaiKhoan", new { area = "" });

        ViewBag.User       = SessionHelper.GetCurrentUser(HttpContext.Session);
        ViewBag.TrangThai  = trangThai;
        var dsDon = await _donRepo.GetAllAsync(trangThai);
        return View(dsDon);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DuyetDon(string maDon)
    {
        if (!IsQuanLy()) return Forbid();
        await _donRepo.DuyetDonAsync(maDon);
        TempData["Success"] = "Đã duyệt đơn thành công.";
        return RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> HuyDon(string maDon)
    {
        if (!IsQuanLy()) return Forbid();
        await _donRepo.HuyDonAdminAsync(maDon);
        TempData["Success"] = "Đã hủy đơn.";
        return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> ChiTiet(string maDon)
    {
        if (!IsQuanLy()) return Forbid();
        ViewBag.User = SessionHelper.GetCurrentUser(HttpContext.Session);
        var don = await _donRepo.GetByIdAsync(maDon);
        if (don is null) return NotFound();
        return View(don);
    }
}
