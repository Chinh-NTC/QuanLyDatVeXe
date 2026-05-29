namespace QLDatVeXe.Domain.Constants;

/// <summary>
/// Các trạng thái của Chuyến xe (CHUYENXE.TrangThai).
/// </summary>
public static class TrangThaiChuyen
{
    /// <summary>Chuyến chưa xuất phát, còn trong tương lai.</summary>
    public const string SapDi      = "SAPDI";

    /// <summary>Chuyến đang trên đường (đã qua giờ xuất phát, chưa đến giờ đến).</summary>
    public const string DangDi     = "DANGDI";

    /// <summary>Chuyến đã đến đích (đã qua giờ dự kiến đến).</summary>
    public const string HoanThanh  = "HOANTHANH";

    /// <summary>Chuyến bị hủy bởi admin/quản lý.</summary>
    public const string Huy        = "HUY";

    /// <summary>Chuyến đã hoàn tất (alias dùng trong DB cũ).</summary>
    public const string Hoan       = "HOAN";
}
