using QLDatVeXe.Validation;
using System.ComponentModel.DataAnnotations;

namespace QLDatVeXe.Domain.DTOs;

public class LoginDTO
{
    [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập")]
    [NotWhitespace(ErrorMessage = "Tên đăng nhập không được chứa toàn khoảng trắng")]
    public string TenDangNhap { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
    [NotWhitespace(ErrorMessage = "Mật khẩu không được chứa toàn khoảng trắng")]
    public string MatKhau     { get; set; } = string.Empty;
}
