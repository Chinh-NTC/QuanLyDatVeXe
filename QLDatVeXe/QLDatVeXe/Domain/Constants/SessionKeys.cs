namespace QLDatVeXe.Domain.Constants;

/// <summary>
/// Các key dùng để lưu thông tin đăng nhập vào Session.
/// </summary>
public static class SessionKeys
{
    public const string VaiTro       = "VaiTro";       // VaiTro enum (int)
    public const string MaTaiKhoan   = "MaTaiKhoan";   // maKH hoặc maNV
    public const string TenDangNhap  = "TenDangNhap";
    public const string HoTen        = "HoTen";
    public const string Sdt          = "Sdt";

    // Booking flow session keys
    public const string BookingMaChuyen   = "Booking_MaChuyen";
    public const string BookingDsGhe      = "Booking_DsGhe";      // JSON array of maGhe
    public const string BookingTenNguoiDi = "Booking_TenNguoiDi";
    public const string BookingSdtNguoiDi = "Booking_SdtNguoiDi";
    public const string BookingGhiChu     = "Booking_GhiChu";
}
