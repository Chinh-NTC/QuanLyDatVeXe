using System;
using System.Collections.Generic;

namespace QLDatVeXe.Models;

public partial class Phuongxa
{
    public string MaPhuong { get; set; } = null!;

    public string MaTinhNo { get; set; } = null!;

    public string TenPhuong { get; set; } = null!;

    public virtual ICollection<Benxe> Benxe { get; set; } = new List<Benxe>();

    public virtual Tinhthanh MaTinhNoNavigation { get; set; } = null!;

    public virtual ICollection<Nhanvien> Nhanvien { get; set; } = new List<Nhanvien>();

    public virtual ICollection<Nhaxe> Nhaxe { get; set; } = new List<Nhaxe>();
}
