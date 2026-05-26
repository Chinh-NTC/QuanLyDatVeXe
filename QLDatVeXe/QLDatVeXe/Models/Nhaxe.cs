using System;
using System.Collections.Generic;

namespace QLDatVeXe.Models;

public partial class Nhaxe
{
    public string MaNhaXe { get; set; } = null!;

    public string TenNhaXe { get; set; } = null!;

    public string? MaPhuongNo { get; set; }

    public string? Sdt { get; set; }

    public string? Email { get; set; }

    public string? DiaChi { get; set; }

    public virtual Phuongxa? MaPhuongNoNavigation { get; set; }

    public virtual ICollection<Xe> Xe { get; set; } = new List<Xe>();
}
