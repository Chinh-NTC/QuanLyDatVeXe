using System;
using System.Collections.Generic;

namespace QLDatVeXe.Models;

public partial class Tinhthanh
{
    public string MaTinh { get; set; } = null!;

    public string TenTinh { get; set; } = null!;

    public string? Img { get; set; }

    public virtual ICollection<Phuongxa> Phuongxa { get; set; } = new List<Phuongxa>();
}
