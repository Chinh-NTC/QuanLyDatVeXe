using QLDatVeXe.Domain.Constants;
using QLDatVeXe.Domain.DTOs;
using QLDatVeXe.Domain.Enums;

namespace QLDatVeXe.Helpers;

/// <summary>
/// Helper tập trung các thao tác đọc/ghi Session liên quan đến tài khoản đăng nhập.
/// </summary>
public static class SessionHelper
{
    // ─── Ghi session ────────────────────────────────────────────────────────────

    public static void SetLogin(ISession session, UserSessionDTO dto)
    {
        session.SetInt32(SessionKeys.VaiTro,      (int)dto.VaiTro);
        session.SetString(SessionKeys.MaTaiKhoan,  dto.MaTaiKhoan);
        session.SetString(SessionKeys.TenDangNhap, dto.TenDangNhap);
        session.SetString(SessionKeys.HoTen,       dto.HoTen);
        session.SetString(SessionKeys.Sdt,         dto.Sdt ?? string.Empty);
    }

    public static void ClearLogin(ISession session)
    {
        session.Remove(SessionKeys.VaiTro);
        session.Remove(SessionKeys.MaTaiKhoan);
        session.Remove(SessionKeys.TenDangNhap);
        session.Remove(SessionKeys.HoTen);
        session.Remove(SessionKeys.Sdt);
        ClearBooking(session);
    }

    // ─── Đọc session ────────────────────────────────────────────────────────────

    public static bool IsLoggedIn(ISession session)
        => session.GetInt32(SessionKeys.VaiTro).HasValue;

    public static VaiTro? GetVaiTro(ISession session)
    {
        var v = session.GetInt32(SessionKeys.VaiTro);
        return v.HasValue ? (VaiTro)v.Value : null;
    }

    public static string? GetMaTaiKhoan(ISession session)
        => session.GetString(SessionKeys.MaTaiKhoan);

    public static string? GetTenDangNhap(ISession session)
        => session.GetString(SessionKeys.TenDangNhap);

    public static string? GetHoTen(ISession session)
        => session.GetString(SessionKeys.HoTen);

    public static string? GetSdt(ISession session)
        => session.GetString(SessionKeys.Sdt);

    public static UserSessionDTO? GetCurrentUser(ISession session)
    {
        var vaiTro = GetVaiTro(session);
        if (vaiTro is null) return null;
        return new UserSessionDTO
        {
            VaiTro      = vaiTro.Value,
            MaTaiKhoan  = GetMaTaiKhoan(session) ?? string.Empty,
            TenDangNhap = GetTenDangNhap(session) ?? string.Empty,
            HoTen       = GetHoTen(session) ?? string.Empty,
            Sdt         = GetSdt(session)
        };
    }

    // ─── Booking flow ────────────────────────────────────────────────────────────

    public static void SetBookingGhe(ISession session, string maChuyen,
        List<string> dsGhe, string tenNguoiDi, string sdtNguoiDi, string? ghiChu)
    {
        session.SetString(SessionKeys.BookingMaChuyen,   maChuyen);
        session.SetString(SessionKeys.BookingDsGhe,      string.Join(",", dsGhe));
        session.SetString(SessionKeys.BookingTenNguoiDi, tenNguoiDi);
        session.SetString(SessionKeys.BookingSdtNguoiDi, sdtNguoiDi);
        session.SetString(SessionKeys.BookingGhiChu,     ghiChu ?? string.Empty);
    }

    public static (string? MaChuyen, List<string> DsGhe, string? TenNguoiDi,
                   string? SdtNguoiDi, string? GhiChu) GetBookingGhe(ISession session)
    {
        var maChuyen   = session.GetString(SessionKeys.BookingMaChuyen);
        var raw        = session.GetString(SessionKeys.BookingDsGhe) ?? string.Empty;
        var dsGhe      = raw.Length > 0 ? raw.Split(',').ToList() : new List<string>();
        var ten        = session.GetString(SessionKeys.BookingTenNguoiDi);
        var sdt        = session.GetString(SessionKeys.BookingSdtNguoiDi);
        var ghiChu     = session.GetString(SessionKeys.BookingGhiChu);
        return (maChuyen, dsGhe, ten, sdt, ghiChu);
    }

    public static void ClearBooking(ISession session)
    {
        session.Remove(SessionKeys.BookingMaChuyen);
        session.Remove(SessionKeys.BookingDsGhe);
        session.Remove(SessionKeys.BookingTenNguoiDi);
        session.Remove(SessionKeys.BookingSdtNguoiDi);
        session.Remove(SessionKeys.BookingGhiChu);
    }
}
