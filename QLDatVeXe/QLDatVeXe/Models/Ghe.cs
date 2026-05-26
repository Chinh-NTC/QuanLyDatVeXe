using System;
using System.Collections.Generic;

namespace QLDatVeXe.Models;

public partial class Ghe
{
    public string MaGhe { get; set; } = null!;

    public string BienSo { get; set; } = null!;

    public string SoGhe { get; set; } = null!;

    public int? Tang { get; set; }

    public string? TrangThai { get; set; }

    public virtual Xe BienSoNavigation { get; set; } = null!;

    public virtual ICollection<Chitietdatve> Chitietdatve { get; set; } = new List<Chitietdatve>();
}
