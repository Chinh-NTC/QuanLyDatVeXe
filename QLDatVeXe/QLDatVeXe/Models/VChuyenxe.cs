using System;
using System.Collections.Generic;

namespace QLDatVeXe.Models;

public partial class VChuyenxe
{
    public string MaChuyen { get; set; } = null!;

    public string? TrangThaiChuyen { get; set; }

    public string MaBenDi { get; set; } = null!;

    public string BenDi { get; set; } = null!;

    public string? PhuongDi { get; set; }

    public string? MaTinhDi { get; set; }

    public string? TinhDi { get; set; }

    public string MaBenDen { get; set; } = null!;

    public string BenDen { get; set; } = null!;

    public string? PhuongDen { get; set; }

    public string? MaTinhDen { get; set; }

    public string? TinhDen { get; set; }

    public DateOnly NgayDi { get; set; }

    public TimeOnly GioDi { get; set; }

    public decimal GiaVe { get; set; }

    public string? LoaiXe { get; set; }

    public string? ImgXe { get; set; }

    public string TenNhaXe { get; set; } = null!;

    public decimal KhoangCach { get; set; }

    public int ThoiGianDuKien { get; set; }

    public int? TongGheTot { get; set; }

    public int? SoGheDaDat { get; set; }

    public int? SoGheTrong { get; set; }
}
