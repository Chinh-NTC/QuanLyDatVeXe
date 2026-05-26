using System;
using System.Collections.Generic;

namespace QLDatVeXe.Models;

public partial class Nhanvien
{
    public string MaNv { get; set; } = null!;

    public string TenDangNhap { get; set; } = null!;

    public string MatKhau { get; set; } = null!;

    public string HoTen { get; set; } = null!;

    public string? Sdt { get; set; }

    public string? Email { get; set; }

    public string? DiaChi { get; set; }

    public string? MaPhuongNo { get; set; }

    public string? ChucVu { get; set; }

    public decimal? Luong { get; set; }

    public DateOnly? NgayVaoLam { get; set; }

    public string? TrangThai { get; set; }

    public virtual ICollection<Dondatve> Dondatve { get; set; } = new List<Dondatve>();

    public virtual Phuongxa? MaPhuongNoNavigation { get; set; }
}
