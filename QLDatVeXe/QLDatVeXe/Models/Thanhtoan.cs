using System;
using System.Collections.Generic;

namespace QLDatVeXe.Models;

public partial class Thanhtoan
{
    public string MaTt { get; set; } = null!;

    public string MaDon { get; set; } = null!;

    public decimal SoTien { get; set; }

    public string? PhuongThuc { get; set; }

    public DateTime? ThoiGianTt { get; set; }

    public string? TrangThai { get; set; }

    public virtual Dondatve MaDonNavigation { get; set; } = null!;
}
