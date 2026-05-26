using System;
using System.Collections.Generic;

namespace QLDatVeXe.Models;

public partial class DondatveKhuyenmai
{
    public string MaDon { get; set; } = null!;

    public string MaKm { get; set; } = null!;

    public decimal? SoTienGiam { get; set; }

    public virtual Dondatve MaDonNavigation { get; set; } = null!;

    public virtual Khuyenmai MaKmNavigation { get; set; } = null!;
}
