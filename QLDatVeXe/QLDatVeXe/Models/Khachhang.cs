using System;
using System.Collections.Generic;

namespace QLDatVeXe.Models;

public partial class Khachhang
{
    public string MaKh { get; set; } = null!;

    public string TenDangNhap { get; set; } = null!;

    public string MatKhau { get; set; } = null!;

    public string HoTen { get; set; } = null!;

    public string Sdt { get; set; } = null!;

    public bool? GioiTinh { get; set; }

    public DateTime? NgayTao { get; set; }

    public bool Trangthai { get; set; }

    public virtual ICollection<Danhgia> Danhgia { get; set; } = new List<Danhgia>();

    public virtual ICollection<Dondatve> Dondatve { get; set; } = new List<Dondatve>();
}
