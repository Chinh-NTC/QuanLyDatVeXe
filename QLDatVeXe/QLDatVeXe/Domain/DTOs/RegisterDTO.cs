using QLDatVeXe.Validation;
using System.ComponentModel.DataAnnotations;

namespace QLDatVeXe.Domain.DTOs;

public class RegisterDTO
{
    [Required(ErrorMessage = "Tên đăng nhập không được để trống")]
    [NotWhitespace(ErrorMessage = "Tên đăng nhập không được chứa toàn khoảng trắng")]
    [StringLength(50, MinimumLength = 5, ErrorMessage = "Tên đăng nhập từ 5-50 ký tự")]
    public string  TenDangNhap   { get; set; } = string.Empty;

    [Required(ErrorMessage = "Mật khẩu không được để trống")]
    [NotWhitespace(ErrorMessage = "Mật khẩu không được chứa toàn khoảng trắng")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu phải từ 6 ký tự trở lên")]
    public string  MatKhau       { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng xác nhận mật khẩu")]
    [NotWhitespace(ErrorMessage = "Xác nhận mật khẩu không được chứa toàn khoảng trắng")]
    [Compare("MatKhau", ErrorMessage = "Mật khẩu xác nhận không khớp")]
    public string  XacNhanMatKhau{ get; set; } = string.Empty;

    [Required(ErrorMessage = "Họ tên không được để trống")]
    [NotWhitespace(ErrorMessage = "Họ tên không được chứa toàn khoảng trắng")]
    [MaxLength(100, ErrorMessage = "Họ tên quá dài")]
    public string  HoTen         { get; set; } = string.Empty;

    [Required(ErrorMessage = "Số điện thoại không được để trống")]
    [RegularExpression(@"^(03|05|07|08|09)\d{8}$", ErrorMessage = "Số điện thoại không hợp lệ (VD: 0912345678)")]
    public string  Sdt           { get; set; } = string.Empty;

    public bool?   GioiTinh      { get; set; }
}
