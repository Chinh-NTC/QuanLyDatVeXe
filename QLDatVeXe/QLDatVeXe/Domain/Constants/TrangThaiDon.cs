namespace QLDatVeXe.Domain.Constants;

/// <summary>
/// Các trạng thái của Đơn đặt vé (DONDATVE.TrangThai)
/// và của Chi tiết đặt vé (CHITIETDATVE.TrangThaiVe).
/// </summary>
public static class TrangThaiDon
{
    /// <summary>Đơn vừa tạo, chờ nhân viên/hệ thống xác nhận.</summary>
    public const string ChoXuLy   = "CHOXULY";

    /// <summary>Đơn đã thanh toán / đã xác nhận thành công.</summary>
    public const string ThanhCong = "THANHCONG";

    /// <summary>Đơn đã bị hủy (bởi khách hoặc admin).</summary>
    public const string DaHuy     = "DAHUY";
}

/// <summary>
/// Trạng thái của từng vé (CHITIETDATVE.TrangThaiVe).
/// </summary>
public static class TrangThaiVe
{
    /// <summary>Ghế đã được đặt thành công.</summary>
    public const string DaDat = "DADAT";

    /// <summary>Vé đã bị hủy.</summary>
    public const string DaHuy = "DAHUY";
}

/// <summary>
/// Trạng thái thanh toán (THANHTOAN.TrangThai).
/// </summary>
public static class TrangThaiThanhToan
{
    /// <summary>Thanh toán thành công.</summary>
    public const string ThanhCong = "THANHCONG";
}
