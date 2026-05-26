using System;
using System.Collections.Generic;

namespace QLDatVeXe.Models;

public partial class Danhgia
{
    public string MaDanhGia { get; set; } = null!;

    public string MaKh { get; set; } = null!;

    public string MaChuyen { get; set; } = null!;

    public byte? DiemDanhGia { get; set; }

    public string? BinhLuan { get; set; }

    public DateTime? NgayDanhGia { get; set; }

    public virtual Chuyenxe MaChuyenNavigation { get; set; } = null!;

    public virtual Khachhang MaKhNavigation { get; set; } = null!;
}
