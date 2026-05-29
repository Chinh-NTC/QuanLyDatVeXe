using QLDatVeXe.Domain.Enums;

namespace QLDatVeXe.Domain.DTOs;

/// <summary>
/// Thông tin session sau đăng nhập — truyền xuống View.
/// </summary>
public class UserSessionDTO
{
    public VaiTro  VaiTro      { get; set; }
    public string  MaTaiKhoan  { get; set; } = string.Empty;
    public string  TenDangNhap { get; set; } = string.Empty;
    public string  HoTen       { get; set; } = string.Empty;
    public string? Sdt         { get; set; }
}
