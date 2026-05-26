using System;
using System.Collections.Generic;

namespace QLDatVeXe.Models;

public partial class Chuyenxe
{
    public string MaChuyen { get; set; } = null!;

    public string MaTuyen { get; set; } = null!;

    public string BienSo { get; set; } = null!;

    public DateOnly NgayDi { get; set; }

    public TimeOnly GioDi { get; set; }

    public decimal GiaVe { get; set; }

    public string? TrangThai { get; set; }

    public virtual Xe BienSoNavigation { get; set; } = null!;

    public virtual ICollection<Chitietdatve> Chitietdatve { get; set; } = new List<Chitietdatve>();

    public virtual ICollection<Danhgia> Danhgia { get; set; } = new List<Danhgia>();

    public virtual Tuyenduong MaTuyenNavigation { get; set; } = null!;
}
