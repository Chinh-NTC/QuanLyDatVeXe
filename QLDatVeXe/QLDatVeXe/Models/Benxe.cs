using System;
using System.Collections.Generic;

namespace QLDatVeXe.Models;

public partial class Benxe
{
    public string MaBenXe { get; set; } = null!;

    public string MaPhuongNo { get; set; } = null!;

    public string TenBenXe { get; set; } = null!;

    public string DiaChi { get; set; } = null!;

    public string? Sdt { get; set; }

    public virtual Phuongxa MaPhuongNoNavigation { get; set; } = null!;

    public virtual ICollection<Tuyenduong> TuyenduongMaBenDenNavigation { get; set; } = new List<Tuyenduong>();

    public virtual ICollection<Tuyenduong> TuyenduongMaBenDiNavigation { get; set; } = new List<Tuyenduong>();
}
