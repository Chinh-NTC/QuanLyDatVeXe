using System;
using System.Collections.Generic;

namespace QLDatVeXe.Models;

public partial class Xe
{
    public string BienSo { get; set; } = null!;

    public string MaNhaXe { get; set; } = null!;

    public string? LoaiXe { get; set; }

    public string? HangXe { get; set; }

    public int? NamSx { get; set; }

    public int? SoTang { get; set; }

    public string? TrangThai { get; set; }

    public string? Img { get; set; }

    public virtual ICollection<Chuyenxe> Chuyenxe { get; set; } = new List<Chuyenxe>();

    public virtual ICollection<Ghe> Ghe { get; set; } = new List<Ghe>();

    public virtual Nhaxe MaNhaXeNavigation { get; set; } = null!;
}
