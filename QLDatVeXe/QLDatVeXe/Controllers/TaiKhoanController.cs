using Microsoft.AspNetCore.Mvc;
using QLDatVeXe.Domain.Constants;
using QLDatVeXe.Domain.DTOs;
using QLDatVeXe.Domain.Enums;
using QLDatVeXe.Helpers;
using QLDatVeXe.Models;
using QLDatVeXe.Repositories.Interfaces;

namespace QLDatVeXe.Controllers;

public class TaiKhoanController : Controller
{
    private readonly IKhachHangRepository _khRepo;
    private readonly INhanVienRepository  _nvRepo;
    private readonly IDonDatVeRepository  _donRepo;
    private readonly IChuyenXeRepository  _chuyenXeRepo;
    private readonly QldatVeXeContext     _db;

    public TaiKhoanController(IKhachHangRepository khRepo, INhanVienRepository nvRepo, IDonDatVeRepository donRepo, IChuyenXeRepository chuyenXeRepo, QldatVeXeContext db)
    {
        _khRepo = khRepo;
        _nvRepo = nvRepo;
        _donRepo = donRepo;
        _chuyenXeRepo = chuyenXeRepo;
        _db = db;
    }

    private bool IsLoggedIn() => SessionHelper.IsLoggedIn(HttpContext.Session);
    private string? MaKH => SessionHelper.GetMaTaiKhoan(HttpContext.Session);

    // ── ĐĂNG NHẬP ────────────────────────────────────────────────────────────

    [HttpGet]
    [Route("Login")]
    [Route("TaiKhoan/Login")]
    public IActionResult Login(string? returnUrl = null)
    {
        if (IsLoggedIn()) return RedirectBasedOnRole();
        ViewBag.ReturnUrl = returnUrl;
        return View("~/Views/TaiKhoan/Login.cshtml");
    }

    [HttpPost]
    [Route("Login")]
    public async Task<IActionResult> LoginPost(string tenDangNhap, string matKhau, string? returnUrl = null)
    {
        // 1. Kiểm tra KHACHHANG
        var kh = await _khRepo.GetByTenDangNhapAsync(tenDangNhap);
        if (kh is not null && !string.IsNullOrEmpty(kh.MatKhau) && kh.MatKhau.Trim() == matKhau?.Trim())
        {
            if (!kh.Trangthai)
            {
                ViewBag.Error = "Tài khoản của bạn đã bị khóa.";
                return View("~/Views/TaiKhoan/Login.cshtml");
            }

            SessionHelper.SetLogin(HttpContext.Session, new UserSessionDTO
            {
                VaiTro      = VaiTro.KhachHang,
                MaTaiKhoan  = kh.MaKh,
                TenDangNhap = kh.TenDangNhap,
                HoTen       = kh.HoTen,
                Sdt         = kh.Sdt
            });
            return !string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl) ? Redirect(returnUrl) : RedirectToAction("Index", "Home");
        }

        // 2. Kiểm tra NHANVIEN
        var nv = await _nvRepo.GetByTenDangNhapAsync(tenDangNhap);
        if (nv is not null && !string.IsNullOrEmpty(nv.MatKhau) && nv.MatKhau.Trim() == matKhau?.Trim() && nv.TrangThai == TrangThaiNhanVien.DangLam)
        {
            var vaiTro = (nv.MaNv.StartsWith("QL_") || 
                          nv.ChucVu?.Trim().Equals("Quản lý", StringComparison.OrdinalIgnoreCase) == true)
                ? VaiTro.QuanLy : VaiTro.NhanVien;

            SessionHelper.SetLogin(HttpContext.Session, new UserSessionDTO
            {
                VaiTro      = vaiTro,
                MaTaiKhoan  = nv.MaNv,
                TenDangNhap = nv.TenDangNhap,
                HoTen       = nv.HoTen,
                Sdt         = nv.Sdt
            });

            return vaiTro == VaiTro.QuanLy
                ? RedirectToAction("Index", "Dashboard", new { area = "management" })
                : RedirectToAction("Index", "NhanVien",  new { area = "management" });
        }

        ViewBag.ReturnUrl = returnUrl;
        ViewBag.Error = "Tên đăng nhập hoặc mật khẩu không đúng.";
        return View("~/Views/TaiKhoan/Login.cshtml");
    }

    // ── ĐĂNG KÝ ──────────────────────────────────────────────────────────────

    [HttpGet]
    [Route("Register")]
    [Route("TaiKhoan/Register")]
    public IActionResult Register(string? returnUrl = null)
    {
        if (IsLoggedIn()) return RedirectToAction("Index", "Home");
        ViewBag.ReturnUrl = returnUrl;
        return View("~/Views/TaiKhoan/Register.cshtml");
    }

    [HttpPost]
    [Route("Register")]
    public async Task<IActionResult> RegisterPost(string hoTen, string sdt, string tenDangNhap, string matKhau, string xacNhanMatKhau, int? gioiTinh, string? returnUrl)
    {
        if (matKhau != xacNhanMatKhau)
        {
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.Error = "Mật khẩu xác nhận không khớp.";
            ViewBag.ActiveTab = "register";
            return View("~/Views/TaiKhoan/Register.cshtml");
        }

        if (await _khRepo.TenDangNhapTonTaiAsync(tenDangNhap))
        {
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.Error = "Tên đăng nhập đã tồn tại.";
            ViewBag.ActiveTab = "register";
            return View("~/Views/TaiKhoan/Register.cshtml");
        }

        if (await _khRepo.SdtTonTaiAsync(sdt))
        {
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.Error = "Số điện thoại đã được đăng ký.";
            ViewBag.ActiveTab = "register";
            return View("~/Views/TaiKhoan/Register.cshtml");
        }

        var kh = new Khachhang
        {
            TenDangNhap = tenDangNhap,
            MatKhau     = matKhau,
            HoTen       = hoTen,
            Sdt         = sdt,
            GioiTinh    = gioiTinh == 1,
            NgayTao     = DateTime.Now,
            Trangthai   = true
        };
        await _khRepo.TaoMoiAsync(kh);

        SessionHelper.SetLogin(HttpContext.Session, new UserSessionDTO
        {
            VaiTro      = VaiTro.KhachHang,
            MaTaiKhoan  = kh.MaKh,
            TenDangNhap = kh.TenDangNhap,
            HoTen       = kh.HoTen,
            Sdt         = kh.Sdt
        });

        return !string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl) ? Redirect(returnUrl) : RedirectToAction("Index", "Home");
    }

    // ── QUẢN LÝ TÀI KHOẢN (ACCOUNT) ───────────────────────────────────────────

    [HttpGet]
    [Route("Account")]
    [Route("TaiKhoan/Account")]
    public async Task<IActionResult> Account(string? tab)
    {
        if (!IsLoggedIn()) return RedirectToAction("Login");
        if (MaKH == null) return RedirectBasedOnRole();

        ViewData["Tab"] = tab ?? "orders";

        var kh = await _khRepo.GetByIdAsync(MaKH);
        ViewBag.KhachHang = kh;

        var orders = await _donRepo.GetLichSuAsync(MaKH);
        ViewBag.Orders = orders; // GetLichSuAsync returns orders with ChiTietDatVe included if possible

        return View("~/Views/TaiKhoan/Account.cshtml");
    }

    [HttpPost]
    [Route("Account")]
    public async Task<IActionResult> AccountPost(string hoTen, string sdt)
    {
        if (!IsLoggedIn()) return RedirectToAction("Login");
        if (MaKH == null) return RedirectBasedOnRole();

        var kh = await _khRepo.GetByIdAsync(MaKH);
        if (kh != null)
        {
            kh.HoTen = hoTen;
            kh.Sdt = sdt;
            await _khRepo.UpdateAsync(kh);
            
            var userSession = SessionHelper.GetCurrentUser(HttpContext.Session);
            if (userSession != null)
            {
                userSession.HoTen = hoTen;
                userSession.Sdt = sdt;
                SessionHelper.SetLogin(HttpContext.Session, userSession);
            }
        }

        return RedirectToAction("Account", new { tab = "profile" });
    }

    // ── TICKET DETAIL ────────────────────────────────────────────────────────

    [HttpGet]
    [Route("TicketDetail")]
    [Route("TaiKhoan/TicketDetail")]
    public async Task<IActionResult> TicketDetail(string maDon)
    {
        if (!IsLoggedIn()) return RedirectToAction("Login");
        if (MaKH == null) return RedirectBasedOnRole();

        var don = await _donRepo.GetByIdAsync(maDon);
        if (don == null || don.MaKh != MaKH) return NotFound();

        ViewBag.DonDatVe = don;

        var ct = don.Chitietdatve?.FirstOrDefault();
        ViewBag.HasReviewed = false;
        if (ct != null)
        {
            var cx = await _chuyenXeRepo.GetChiTietAsync(ct.MaChuyen);
            ViewBag.ChuyenXe = cx;
            
            var existingReview = _db.Danhgia.FirstOrDefault(r => r.MaKh == MaKH && r.MaChuyen == ct.MaChuyen);
            ViewBag.HasReviewed = existingReview != null;
        }

        return View("~/Views/TaiKhoan/TicketDetail.cshtml");
    }

    [HttpPost]
    [Route("TicketDetail")]
    public async Task<IActionResult> TicketDetailPost(string action, string maDon, string? maChuyen, int? rating, string? comment)
    {
        if (!IsLoggedIn()) return RedirectToAction("Login");
        if (MaKH == null) return RedirectBasedOnRole();

        if (action == "cancel")
        {
            var don = await _donRepo.GetByIdAsync(maDon);
            if (don != null && don.MaKh == MaKH)
            {
                var ct = don.Chitietdatve?.FirstOrDefault();
                if (ct != null)
                {
                    var cx = await _chuyenXeRepo.GetChiTietAsync(ct.MaChuyen);
                    if (cx != null)
                    {
                        var departureDateTime = cx.NgayDi.ToDateTime(cx.GioDi);
                        if (DateTime.Now <= departureDateTime.AddHours(-1))
                        {
                            await _donRepo.HuyDonAsync(maDon, MaKH);
                        }
                    }
                }
            }
        }
        else if (action == "review" && maChuyen != null && rating != null)
        {
            var existingReview = _db.Danhgia.FirstOrDefault(r => r.MaKh == MaKH && r.MaChuyen == maChuyen);
            if (existingReview != null)
            {
                existingReview.DiemDanhGia = (byte)rating;
                existingReview.BinhLuan = comment;
                existingReview.NgayDanhGia = DateTime.Now;
                _db.Danhgia.Update(existingReview);
            }
            else
            {
                var review = new Danhgia
                {
                    MaDanhGia = "DG_" + DateTime.Now.ToString("yyyyMMddHHmmss") + new Random().Next(100, 999),
                    MaKh = MaKH,
                    MaChuyen = maChuyen,
                    DiemDanhGia = (byte)rating,
                    BinhLuan = comment,
                    NgayDanhGia = DateTime.Now
                };
                _db.Danhgia.Add(review);
            }
            await _db.SaveChangesAsync();
        }

        return RedirectToAction("Account", new { tab = "orders" });
    }

    // ── ĐĂNG XUẤT ────────────────────────────────────────────────────────────

    [HttpGet]
    [Route("Logout")]
    public IActionResult LogoutGet() // Map to /Logout
    {
        SessionHelper.ClearLogin(HttpContext.Session);
        return RedirectToAction("Login");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Logout()
    {
        SessionHelper.ClearLogin(HttpContext.Session);
        return RedirectToAction("Login");
    }

    // ── HELPER ────────────────────────────────────────────────────────────────

    private IActionResult RedirectBasedOnRole()
    {
        var vaiTro = SessionHelper.GetVaiTro(HttpContext.Session);
        return vaiTro switch
        {
            VaiTro.QuanLy   => RedirectToAction("Index", "Dashboard", new { area = "management" }),
            VaiTro.NhanVien => RedirectToAction("Index", "NhanVien",  new { area = "management" }),
            _               => RedirectToAction("Index", "Home")
        };
    }
}
