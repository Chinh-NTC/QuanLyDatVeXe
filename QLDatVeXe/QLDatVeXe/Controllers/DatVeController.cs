using Microsoft.AspNetCore.Mvc;
using QLDatVeXe.Domain.DTOs;
using QLDatVeXe.Domain.Enums;
using QLDatVeXe.Helpers;
using QLDatVeXe.Repositories.Interfaces;

namespace QLDatVeXe.Controllers;

public class DatVeController : Controller
{
    private readonly IChuyenXeRepository _chuyenXeRepo;
    private readonly IDonDatVeRepository _donRepo;

    public DatVeController(IChuyenXeRepository chuyenXeRepo, IDonDatVeRepository donRepo)
    {
        _chuyenXeRepo = chuyenXeRepo;
        _donRepo      = donRepo;
    }

    // ── Kiểm tra đăng nhập là KhachHang ──────────────────────────────────────
    private bool IsKhachHang() =>
        SessionHelper.GetVaiTro(HttpContext.Session) == VaiTro.KhachHang;

    private string? MaKH => SessionHelper.GetMaTaiKhoan(HttpContext.Session);

    // ── 1. TÌM CHUYẾN ─────────────────────────────────────────────────────────

    [HttpGet]
    [Route("Search")]
    [Route("DatVe/ChonChuyen")]
    public async Task<IActionResult> ChonChuyen(string? maTinhDi, string? maTinhDen, string? ngayDi)
    {
        ViewBag.User     = SessionHelper.GetCurrentUser(HttpContext.Session);
        ViewBag.DsTinhThanh   = await _chuyenXeRepo.GetAllTinhThanhAsync();
        ViewBag.MaTinhDi = maTinhDi;
        ViewBag.MaTinhDen= maTinhDen;

        DateOnly searchDate = DateOnly.FromDateTime(DateTime.Now);
        if (!string.IsNullOrEmpty(ngayDi) && DateOnly.TryParseExact(ngayDi, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out var nd))
        {
            searchDate = nd;
        }

        // Lấy tất cả chuyến hợp lệ cho tuyến này
        var allChuyen = await _chuyenXeRepo.TimChuyenAsync(maTinhDi, maTinhDen, null);
        var now = DateTime.Now;
        var todayDate = DateOnly.FromDateTime(now);
        var currentTime = TimeOnly.FromDateTime(now);
        
        // Chỉ lấy các chuyến trong tương lai
        var validTrips = allChuyen.Where(c => c.NgayDi > todayDate || (c.NgayDi == todayDate && c.GioDi > currentTime))
                                  .OrderBy(c => c.NgayDi).ThenBy(c => c.GioDi).ToList();

        // Lấy chuyến trong ngày tìm kiếm
        var dsChuyen = validTrips.Where(c => c.NgayDi == searchDate).ToList();
        
        // Nếu không có chuyến nào vào ngày này, tìm ngày tiếp theo có chuyến
        if (!dsChuyen.Any() && validTrips.Any(c => c.NgayDi > searchDate))
        {
            var nextDate = validTrips.Where(c => c.NgayDi > searchDate).Min(c => c.NgayDi);
            dsChuyen = validTrips.Where(c => c.NgayDi == nextDate).ToList();
            ViewBag.NgayDi = nextDate.ToString("yyyy-MM-dd");
            ViewBag.SearchMessage = $"Không có chuyến nào vào ngày {searchDate:dd/MM/yyyy}. Đang hiển thị các chuyến vào ngày {nextDate:dd/MM/yyyy} gần nhất.";
        }
        else
        {
            ViewBag.NgayDi = searchDate.ToString("yyyy-MM-dd");
        }

        ViewBag.Results = dsChuyen;
        return View("~/Views/DatVe/ChonChuyen.cshtml");
    }

    // ── 2. CHỌN GHẾ ───────────────────────────────────────────────────────────

    [HttpGet]
    [Route("Booking")]
    [Route("DatVe/ChonGhe")]
    public async Task<IActionResult> ChonGhe(string maChuyen)
    {
        if (!IsKhachHang())
            return RedirectToAction("Login", "TaiKhoan", new { returnUrl = Url.Action("ChonGhe", new { maChuyen }) });

        var chuyenXe = await _chuyenXeRepo.GetChiTietAsync(maChuyen);
        if (chuyenXe is null) return NotFound();

        var dsGhe = await _chuyenXeRepo.GetDanhSachGheAsync(maChuyen);

        ViewBag.User     = SessionHelper.GetCurrentUser(HttpContext.Session);
        ViewBag.ChuyenXe = chuyenXe;
        ViewBag.DanhSachGhe = dsGhe;

        var gheDaDat = dsGhe.Where(g => g.TrangThai == "DADAT").Select(g => g.MaGhe).ToList();
        ViewBag.GheDaDat = gheDaDat;
        
        // Truyền thông tin người dùng
        ViewBag.UserPhone = SessionHelper.GetSdt(HttpContext.Session);
        ViewBag.UserName = SessionHelper.GetHoTen(HttpContext.Session);
        
        return View("~/Views/DatVe/ChonGhe.cshtml");
    }

    // ── 3. LƯU CHỌN GHẾ + CHUYỂN SANG THANH TOÁN (POST) ────────────────────────────

    [HttpPost]
    [Route("Payment")]
    [Route("DatVe/ThanhToan")]
    public async Task<IActionResult> ThanhToan(string maChuyen, string maGhe, string sdtKhach, string hoTenKhach, string isOtherPerson, string hoTenNguoiDi, string sdtNguoiDi)
    {
        if (!IsKhachHang())
            return RedirectToAction("Login", "TaiKhoan");

        if (string.IsNullOrEmpty(maChuyen) || string.IsNullOrEmpty(maGhe))
            return RedirectToAction("ChonGhe", new { maChuyen = maChuyen });

        var chuyenXe = await _chuyenXeRepo.GetChiTietAsync(maChuyen);
        if (chuyenXe == null) return NotFound();

        ViewBag.ChuyenXe = chuyenXe;
        ViewBag.MaGhes = maGhe;
        
        int soGhe = maGhe.Split(',', StringSplitOptions.RemoveEmptyEntries).Length;
        ViewBag.SoGhe = soGhe;
        ViewBag.TongTien = soGhe * chuyenXe.GiaVe;

        ViewBag.SdtKhach = sdtKhach;
        ViewBag.HoTenKhach = hoTenKhach;
        
        if (isOtherPerson == "true")
        {
            ViewBag.HoTenNguoiDi = hoTenNguoiDi;
            ViewBag.SdtNguoiDi = sdtNguoiDi;
        }
        else
        {
            ViewBag.HoTenNguoiDi = hoTenKhach;
            ViewBag.SdtNguoiDi = sdtKhach;
        }

        ViewBag.KhuyenMais = await _donRepo.GetUnusedKhuyenMaiAsync(MaKH!);

        return View("~/Views/DatVe/ThanhToan.cshtml");
    }

    // ── 4. XÁC NHẬN ĐẶT VÉ ────────────────────────────────────────────

    [HttpPost]
    [Route("ConfirmPayment")]
    [Route("DatVe/ConfirmPayment")]
    public async Task<IActionResult> XacNhanDatVe(string maChuyen, string maGhe, string hoTenNguoiDi, string sdtNguoiDi, string phuongThuc, string? maKM)
    {
        if (!IsKhachHang())
            return RedirectToAction("Login", "TaiKhoan");

        if (string.IsNullOrEmpty(maChuyen) || string.IsNullOrEmpty(maGhe))
            return RedirectToAction("ChonChuyen");

        var dto = new DatVeDTO
        {
            MaChuyen = maChuyen,
            DsGhe = maGhe.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(g => g.Trim()).ToList(),
            GhiChu = "",
            MaKM = maKM,
            PhuongThuc = phuongThuc == "tien-mat" ? "TIENMAT" : "CHUYENKHOAN",
            TenNguoiDi = hoTenNguoiDi,
            SdtNguoiDi = sdtNguoiDi
        };

        var result = await _donRepo.DatVeAsync(dto, MaKH!);
        if (result.ThanhCong)
        {
            TempData["Success"] = "Chúc mừng! Bạn đã đặt vé thành công.";
            return RedirectToAction("Index", "Home");
        }
        else
        {
            TempData["Error"] = result.ThongBao;
            return RedirectToAction("ChonGhe", new { maChuyen = maChuyen });
        }
    }

    // ── 7. HỦY ĐƠN ───────────────────────────────────────────────────────────

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> HuyDon(string maDon)
    {
        if (!IsKhachHang())
            return RedirectToAction("Login", "TaiKhoan");

        var ok = await _donRepo.HuyDonAsync(maDon, MaKH!);
        TempData[ok ? "Success" : "Error"] = ok
            ? "Đã hủy đơn thành công."
            : "Không thể hủy đơn này (chỉ hủy được đơn đang chờ xử lý).";

        return Redirect("/Account?tab=orders");
    }
}
