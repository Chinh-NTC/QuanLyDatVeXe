namespace QLDatVeXe.Domain.DTOs;

/// <summary>
/// DTO cho luồng đặt vé (hỗ trợ đặt hộ: TenNguoiDi / SdtNguoiDi có thể khác tài khoản đăng nhập).
/// </summary>
public class DatVeDTO
{
    public string        MaChuyen     { get; set; } = string.Empty;
    public List<string>  DsGhe        { get; set; } = new();   // danh sách maGhe đã chọn
    public string        TenNguoiDi   { get; set; } = string.Empty;
    public string        SdtNguoiDi   { get; set; } = string.Empty;
    public string        PhuongThuc   { get; set; } = "TIENMAT"; // TIENMAT | CHUYENKHOAN
    public string?       GhiChu       { get; set; }
    public string?       MaKM         { get; set; }  // mã khuyến mãi (tuỳ chọn)
}
