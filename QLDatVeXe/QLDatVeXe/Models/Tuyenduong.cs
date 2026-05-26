using System;
using System.Collections.Generic;

namespace QLDatVeXe.Models;

public partial class Tuyenduong
{
    public string MaTuyen { get; set; } = null!;

    public string MaBenDi { get; set; } = null!;

    public string MaBenDen { get; set; } = null!;

    public decimal KhoangCach { get; set; }

    public int ThoiGianDuKien { get; set; }

    public virtual ICollection<Chuyenxe> Chuyenxe { get; set; } = new List<Chuyenxe>();

    public virtual Benxe MaBenDenNavigation { get; set; } = null!;

    public virtual Benxe MaBenDiNavigation { get; set; } = null!;
}
