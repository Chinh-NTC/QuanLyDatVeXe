using QLDatVeXe.Models;

namespace QLDatVeXe.Domain.Business;

/// <summary>
/// Xử lý các nghiệp vụ liên quan đến đặt vé: tính tiền, validate, sinh mã đơn.
/// </summary>
public static class DatVeBusiness
{
    /// <summary>Sinh mã đơn dạng DON + yyyyMMddHHmmss + 4 ký tự ngẫu nhiên</summary>
    public static string SinhMaDon()
    {
        var stamp = DateTime.Now.ToString("yyyyMMddHHmmss");
        var rand  = Guid.NewGuid().ToString("N")[..4].ToUpper();
        return $"DON{stamp}{rand}";
    }

    /// <summary>Sinh mã chi tiết đặt vé</summary>
    public static string SinhMaCtDat()
    {
        var stamp = DateTime.Now.ToString("yyyyMMddHHmmssff");
        var rand  = Guid.NewGuid().ToString("N")[..4].ToUpper();
        return $"CT{stamp}{rand}";
    }

    /// <summary>Sinh mã thanh toán</summary>
    public static string SinhMaTT()
    {
        var stamp = DateTime.Now.ToString("yyyyMMddHHmmssff");
        var rand  = Guid.NewGuid().ToString("N")[..4].ToUpper();
        return $"TT{stamp}{rand}";
    }

    /// <summary>
    /// Tính tổng tiền sau khi áp dụng khuyến mãi.
    /// </summary>
    public static decimal TinhTongTien(decimal giaVe, int soGhe, Khuyenmai? km)
    {
        decimal tongTien = giaVe * soGhe;
        if (km is null) return tongTien;

        if (string.Equals(km.LoaiKm, "PHANTRAM", StringComparison.OrdinalIgnoreCase))
        {
            decimal giam = Math.Round(tongTien * km.GiaTriGiam / 100m, 0);
            tongTien -= giam;
        }
        else // CODINH
        {
            tongTien = Math.Max(0, tongTien - km.GiaTriGiam);
        }
        return tongTien;
    }

    /// <summary>
    /// Kiểm tra khuyến mãi còn hiệu lực không.
    /// </summary>
    public static bool KhuyenMaiConHieuLuc(Khuyenmai km)
        => km.NgayBatDau <= DateTime.Now && DateTime.Now <= km.NgayKetThuc;
}
