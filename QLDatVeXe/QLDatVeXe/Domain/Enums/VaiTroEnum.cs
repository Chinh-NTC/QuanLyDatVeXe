namespace QLDatVeXe.Domain.Enums;

/// <summary>
/// Biểu thị vai trò / loại tài khoản đăng nhập vào hệ thống.
/// Dùng để phân quyền điều hướng sau khi đăng nhập.
/// </summary>
public enum VaiTro
{
    /// <summary>Khách hàng — đăng ký qua form đăng ký, lưu trong bảng KHACHHANG</summary>
    KhachHang = 1,

    /// <summary>Nhân viên bán vé — lưu trong bảng NHANVIEN, chucVu != "Quản lý"</summary>
    NhanVien = 2,

    /// <summary>Quản lý — lưu trong bảng NHANVIEN, chucVu = "Quản lý"</summary>
    QuanLy = 3
}
