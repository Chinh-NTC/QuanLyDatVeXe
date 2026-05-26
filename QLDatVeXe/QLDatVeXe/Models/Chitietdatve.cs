using System;
using System.Collections.Generic;

namespace QLDatVeXe.Models;

public partial class Chitietdatve
{
    public string MaCtdat { get; set; } = null!;

    public string MaDon { get; set; } = null!;

    public string MaChuyen { get; set; } = null!;

    public string MaGhe { get; set; } = null!;

    public decimal GiaVeLucDat { get; set; }

    public string? TrangThaiVe { get; set; }

    public virtual Chuyenxe MaChuyenNavigation { get; set; } = null!;

    public virtual Dondatve MaDonNavigation { get; set; } = null!;

    public virtual Ghe MaGheNavigation { get; set; } = null!;
}
