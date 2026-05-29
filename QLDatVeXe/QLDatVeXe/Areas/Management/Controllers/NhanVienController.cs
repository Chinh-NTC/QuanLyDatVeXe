using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLDatVeXe.Domain.Constants;
using QLDatVeXe.Domain.Enums;
using QLDatVeXe.Helpers;
using QLDatVeXe.Models;
using QLDatVeXe.Repositories.Interfaces;

namespace QLDatVeXe.Areas.management.Controllers;

[Area("management")]
public class NhanVienController : Controller
{
    private readonly IChuyenXeRepository  _chuyenXeRepo;
    private readonly IKhachHangRepository _khRepo;
    private readonly INhaxeRepository     _nhaxeRepo;
    private readonly IDonDatVeRepository  _donRepo;
    private readonly QldatVeXeContext     _db;

    public NhanVienController(
        IChuyenXeRepository chuyenXeRepo,
        IKhachHangRepository khRepo,
        INhaxeRepository nhaxeRepo,
        IDonDatVeRepository donRepo,
        QldatVeXeContext db)
    {
        _chuyenXeRepo = chuyenXeRepo;
        _khRepo       = khRepo;
        _nhaxeRepo    = nhaxeRepo;
        _donRepo      = donRepo;
        _db           = db;
    }

    private bool IsStaff() =>
        SessionHelper.GetVaiTro(HttpContext.Session) is VaiTro.NhanVien or VaiTro.QuanLy;

    // ── Trang chủ nhân viên ──────────────────────────────────
    public async Task<IActionResult> Index(string? locTrangThai, string? locTrangThaiChuyen, string? maTinhDi, string? maTinhDen)
    {
        if (!IsStaff())
            return RedirectToAction("Login", "TaiKhoan", new { area = "" });

        ViewBag.User         = SessionHelper.GetCurrentUser(HttpContext.Session);
        ViewBag.DsTinh       = await _chuyenXeRepo.GetAllTinhThanhAsync();

        // Thống kê nhanh
        var today = DateOnly.FromDateTime(DateTime.Today);
        ViewBag.ChuyenHomNay  = await _db.Chuyenxe.CountAsync(c => c.NgayDi == today);
        ViewBag.TongDonHomNay = await _db.Dondatve.CountAsync(d =>
            d.NgayDat.HasValue && d.NgayDat.Value.Date == DateTime.Today);

        // Đơn đặt vé hôm nay
        var query = _db.Dondatve
            .Include(d => d.MaKhNavigation)
            .Include(d => d.Chitietdatve)
            .Where(d => d.NgayDat.HasValue && d.NgayDat.Value.Date == DateTime.Today);

        if (!string.IsNullOrEmpty(locTrangThai))
        {
            query = query.Where(d => d.TrangThai == locTrangThai);
        }

        ViewBag.DonHomNay = await query.OrderByDescending(d => d.NgayDat).ToListAsync();
        ViewBag.LocTrangThai = locTrangThai;

        // Các chuyến hôm nay
        var dsChuyen = await _chuyenXeRepo.TimChuyenAsync(maTinhDi, maTinhDen, DateOnly.FromDateTime(DateTime.Today));
        if (!string.IsNullOrEmpty(locTrangThaiChuyen))
        {
            dsChuyen = dsChuyen.Where(c => c.TrangThaiChuyen == locTrangThaiChuyen).ToList();
        }
        ViewBag.ChuyenHomNayList = dsChuyen.OrderBy(c => c.GioDi).ToList();
        ViewBag.LocTrangThaiChuyen = locTrangThaiChuyen;
        ViewBag.MaTinhDi = maTinhDi;
        ViewBag.MaTinhDen = maTinhDen;

        return View();
    }

    // ── Tìm chuyến ──────────────────────────────────────────
    [HttpGet]
    public async Task<IActionResult> TimChuyen(
        string? maTinhDi, string? maTinhDen, string? ngayDi, string? maKhachHang)
    {
        if (!IsStaff())
            return RedirectToAction("Login", "TaiKhoan", new { area = "" });

        ViewBag.DsTinh       = await _chuyenXeRepo.GetAllTinhThanhAsync();
        ViewBag.DsKhachHang  = await _khRepo.GetAllAsync();
        ViewBag.MaTinhDi     = maTinhDi;
        ViewBag.MaTinhDen    = maTinhDen;
        ViewBag.NgayDi       = ngayDi;
        ViewBag.MaKhachHang  = maKhachHang;

        List<VChuyenxe> dsChuyen = new();
        if (!string.IsNullOrEmpty(ngayDi) && DateOnly.TryParse(ngayDi, out var nd))
            dsChuyen = await _chuyenXeRepo.TimChuyenAsync(maTinhDi, maTinhDen, nd);
        else if (!string.IsNullOrEmpty(maTinhDi) || !string.IsNullOrEmpty(maTinhDen))
            dsChuyen = await _chuyenXeRepo.TimChuyenAsync(maTinhDi, maTinhDen, null);
        else 
            dsChuyen = await _chuyenXeRepo.TimChuyenAsync(null, null, null);

        // Ẩn các chuyến đã qua giờ xuất phát
        var now = DateTime.Now;
        dsChuyen = dsChuyen.Where(c => c.NgayDi.ToDateTime(c.GioDi) > now).ToList();

        ViewBag.DsChuyen = dsChuyen;
        return View(dsChuyen);
    }

    // ── API lấy danh sách ghế trống của chuyến ───────────────
    [HttpGet]
    public async Task<IActionResult> LayDanhSachGhe(string maChuyen)
    {
        if (!IsStaff()) return Forbid();

        var chuyen = await _db.Chuyenxe
            .Include(c => c.BienSoNavigation)
                .ThenInclude(x => x.Ghe)
            .FirstOrDefaultAsync(c => c.MaChuyen == maChuyen);

        if (chuyen is null) return NotFound();

        // Ghế đã đặt của chuyến này
        var gheDaDat = await _db.Chitietdatve
            .Where(ct => ct.MaChuyen == maChuyen && ct.TrangThaiVe != TrangThaiVe.DaHuy)
            .Select(ct => ct.MaGhe)
            .ToListAsync();

        var ghes = chuyen.BienSoNavigation.Ghe
            .Select(g => new {
                g.MaGhe,
                g.SoGhe,
                g.Tang,
                TrangThai = gheDaDat.Contains(g.MaGhe) ? TrangThaiVe.DaDat : g.TrangThai
            })
            .OrderBy(g => g.Tang)
            .ThenBy(g => g.SoGhe)
            .ToList();

        return Json(ghes);
    }

    // ── Đặt vé hộ khách ──────────────────────────────────────
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DatVeHo(
        string maChuyen, string maKhachHang, List<string> maGhes,
        decimal giaVeLucDat, string? tenNguoiDi, string? sdtNguoiDi)
    {
        if (!IsStaff())
            return RedirectToAction("Login", "TaiKhoan", new { area = "" });

        var nvUser = SessionHelper.GetCurrentUser(HttpContext.Session);

        // Kiểm tra đầu vào
        if (string.IsNullOrEmpty(maKhachHang) || maGhes == null || !maGhes.Any())
        {
            TempData["Error"] = "Vui lòng chọn khách hàng và ít nhất 1 ghế.";
            return RedirectToAction(nameof(TimChuyen));
        }

        if (maGhes.Count > 5)
        {
            TempData["Error"] = "Chỉ được chọn tối đa 5 ghế.";
            return RedirectToAction(nameof(TimChuyen));
        }

        // Lấy giá vé từ chuyến xe (không tin vào client)
        var chuyen = await _db.Chuyenxe.FindAsync(maChuyen);
        if (chuyen is null)
        {
            TempData["Error"] = "Chuyến xe không tồn tại.";
            return RedirectToAction(nameof(TimChuyen));
        }

        // Kiểm tra ghế còn trống không
        var gheDaDat = await _db.Chitietdatve
            .AnyAsync(ct => ct.MaChuyen == maChuyen && maGhes.Contains(ct.MaGhe) && ct.TrangThaiVe != TrangThaiVe.DaHuy);
        if (gheDaDat)
        {
            TempData["Error"] = "Có ghế đã được người khác đặt. Vui lòng chọn lại.";
            return RedirectToAction(nameof(TimChuyen));
        }

        // Tạo mã đơn mới
        var maxDon = await _db.Dondatve.OrderByDescending(d => d.MaDon).FirstOrDefaultAsync();
        int maDonNum = 1;
        if (maxDon != null && maxDon.MaDon.StartsWith("DV"))
            int.TryParse(maxDon.MaDon.Substring(2), out maDonNum);
        maDonNum++;
        var maDonMoi = $"DV{maDonNum:D4}";

        var don = new Dondatve
        {
            MaDon      = maDonMoi,
            MaKh       = maKhachHang,
            MaNv       = nvUser?.MaTaiKhoan,
            NgayDat    = DateTime.Now,
            TongTien   = chuyen.GiaVe * maGhes.Count,
            TienCoc    = 0,
            TrangThai  = TrangThaiDon.ThanhCong,
            Tennguoidi = string.IsNullOrWhiteSpace(tenNguoiDi) ? null : tenNguoiDi,
            Sdtnguoidi = string.IsNullOrWhiteSpace(sdtNguoiDi) ? null : sdtNguoiDi,
            GhiChu     = "Đặt hộ bởi nhân viên"
        };
        _db.Dondatve.Add(don);

        // Tạo chi tiết đặt vé
        var maxCT = await _db.Chitietdatve.OrderByDescending(c => c.MaCtdat).FirstOrDefaultAsync();
        int maCTNum = 1;
        if (maxCT != null && maxCT.MaCtdat.StartsWith("CT"))
            int.TryParse(maxCT.MaCtdat.Substring(2), out maCTNum);

        foreach (var mg in maGhes)
        {
            maCTNum++;
            var chiTiet = new Chitietdatve
            {
                MaCtdat     = $"CT{maCTNum:D4}",
                MaDon       = maDonMoi,
                MaChuyen    = maChuyen,
                MaGhe       = mg,
                GiaVeLucDat = chuyen.GiaVe,
                TrangThaiVe = TrangThaiVe.DaDat
            };
            _db.Chitietdatve.Add(chiTiet);
        }

        // Thanh toán
        var maxTT = await _db.Thanhtoan.OrderByDescending(t => t.MaTt).FirstOrDefaultAsync();
        int maTTNum = 1;
        if (maxTT != null && maxTT.MaTt.StartsWith("TT"))
            int.TryParse(maxTT.MaTt.Substring(2), out maTTNum);
        maTTNum++;

        var tt = new Thanhtoan
        {
            MaTt        = $"TT{maTTNum:D4}",
            MaDon       = maDonMoi,
            SoTien      = chuyen.GiaVe * maGhes.Count,
            PhuongThuc  = "TIENMAT",
            ThoiGianTt  = DateTime.Now,
            TrangThai   = TrangThaiThanhToan.ThanhCong
        };
        _db.Thanhtoan.Add(tt);

        await _db.SaveChangesAsync();

        TempData["Success"] = $"Đặt vé thành công! Mã đơn: {maDonMoi}";
        return RedirectToAction(nameof(TimChuyen));
    }

    // ── Xem danh sách khách hàng ─────────────────────────────
    [HttpGet]
    public async Task<IActionResult> XemKhachHang(string? search)
    {
        if (!IsStaff())
            return RedirectToAction("Login", "TaiKhoan", new { area = "" });

        ViewBag.Search = search;
        var dsKH = await _khRepo.GetAllAsync();
        if (!string.IsNullOrEmpty(search))
            dsKH = dsKH.Where(k =>
                k.HoTen.Contains(search, StringComparison.OrdinalIgnoreCase)
             || k.Sdt.Contains(search)
             || k.TenDangNhap.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();

        return View(dsKH);
    }

    // ── Xem danh sách nhà xe ─────────────────────────────────
    [HttpGet]
    public async Task<IActionResult> XemNhaxe(string? search)
    {
        if (!IsStaff())
            return RedirectToAction("Login", "TaiKhoan", new { area = "" });

        ViewBag.Search = search;
        var query = _db.Nhaxe.Include(n => n.Xe).AsQueryable();
        if (!string.IsNullOrEmpty(search))
            query = query.Where(n => n.TenNhaXe.Contains(search)
                                  || (n.Sdt != null && n.Sdt.Contains(search)));

        var dsNhaxe = await query.ToListAsync();
        return View(dsNhaxe);
    }
}
