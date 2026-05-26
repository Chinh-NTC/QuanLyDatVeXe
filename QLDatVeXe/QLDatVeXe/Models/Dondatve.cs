using System;
using System.Collections.Generic;

namespace QLDatVeXe.Models;

public partial class Dondatve
{
    public string MaDon { get; set; } = null!;

    public string MaKh { get; set; } = null!;

    public string? MaNv { get; set; }

    public DateTime? NgayDat { get; set; }

    public decimal? TongTien { get; set; }

    public decimal? TienCoc { get; set; }

    public string? TrangThai { get; set; }

    public string? Tennguoidi { get; set; }

    public string? Sdtnguoidi { get; set; }

    public string? GhiChu { get; set; }

    public virtual ICollection<Chitietdatve> Chitietdatve { get; set; } = new List<Chitietdatve>();

    public virtual ICollection<DondatveKhuyenmai> DondatveKhuyenmai { get; set; } = new List<DondatveKhuyenmai>();

    public virtual Khachhang MaKhNavigation { get; set; } = null!;

    public virtual Nhanvien? MaNvNavigation { get; set; }

    public virtual ICollection<Thanhtoan> Thanhtoan { get; set; } = new List<Thanhtoan>();
}
