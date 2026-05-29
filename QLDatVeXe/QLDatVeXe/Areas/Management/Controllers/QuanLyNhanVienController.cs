using Microsoft.AspNetCore.Mvc;
using QLDatVeXe.Domain.Enums;
using QLDatVeXe.Helpers;
using QLDatVeXe.Models;
using QLDatVeXe.Repositories.Interfaces;

namespace QLDatVeXe.Areas.management.Controllers;

[Area("management")]
public class QuanLyNhanVienController : Controller
{
    private readonly INhanVienRepository _nvRepo;

    public QuanLyNhanVienController(INhanVienRepository nvRepo) => _nvRepo = nvRepo;

    private bool IsQuanLy() => SessionHelper.GetVaiTro(HttpContext.Session) == VaiTro.QuanLy;

    public async Task<IActionResult> Index()
    {
        if (!IsQuanLy())
            return RedirectToAction("Login", "TaiKhoan", new { area = "" });

        ViewBag.User = SessionHelper.GetCurrentUser(HttpContext.Session);
        var dsNV = await _nvRepo.GetAllAsync();
        
        // Ẩn quản lý khỏi danh sách
        dsNV = dsNV.Where(nv => nv.ChucVu != "Quản lý").ToList();

        return View(dsNV);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Them(Nhanvien nv, string matKhauMoi)
    {
        if (!IsQuanLy()) return Forbid();

        if (await _nvRepo.TenDangNhapTonTaiAsync(nv.TenDangNhap))
        {
            TempData["Error"] = "Tên đăng nhập đã tồn tại.";
            return RedirectToAction("Index");
        }

        nv.MatKhau = matKhauMoi;
        await _nvRepo.TaoMoiAsync(nv);
        TempData["Success"] = $"Đã thêm nhân viên {nv.HoTen} thành công!";
        return RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Sua(Nhanvien nv, string? matKhauMoi)
    {
        if (!IsQuanLy()) return Forbid();

        var existing = await _nvRepo.GetByIdAsync(nv.MaNv);
        if (existing is null) return NotFound();

        existing.HoTen    = nv.HoTen;
        existing.Sdt      = nv.Sdt;
        existing.Email    = nv.Email;
        existing.DiaChi   = nv.DiaChi;
        existing.ChucVu   = nv.ChucVu;
        existing.Luong    = nv.Luong;
        existing.TrangThai= nv.TrangThai;
        if (!string.IsNullOrWhiteSpace(matKhauMoi))
            existing.MatKhau = matKhauMoi;

        await _nvRepo.UpdateAsync(existing);
        TempData["Success"] = "Cập nhật thông tin nhân viên thành công!";
        return RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Xoa(string maNV)
    {
        if (!IsQuanLy()) return Forbid();
        await _nvRepo.DeleteAsync(maNV); // Set trangThai = NGHIVIEC
        TempData["Success"] = "Đã đặt nhân viên sang trạng thái nghỉ việc.";
        return RedirectToAction("Index");
    }
}
