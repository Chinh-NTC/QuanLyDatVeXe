using System;
using System.Collections.Generic;

namespace QLDatVeXe.Models;

public partial class Khuyenmai
{
    public string MaKm { get; set; } = null!;

    public string TenKhuyenMai { get; set; } = null!;

    public string? LoaiKm { get; set; }

    public decimal GiaTriGiam { get; set; }

    public DateTime NgayBatDau { get; set; }

    public DateTime NgayKetThuc { get; set; }

    public virtual ICollection<DondatveKhuyenmai> DondatveKhuyenmai { get; set; } = new List<DondatveKhuyenmai>();
}
