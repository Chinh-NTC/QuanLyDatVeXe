namespace QLDatVeXe.Domain.DTOs;

/// <summary>
/// Kết quả trả về sau khi đặt vé thành công.
/// </summary>
public class DatVeResultDTO
{
    public bool   ThanhCong  { get; set; }
    public string MaDon      { get; set; } = string.Empty;
    public string ThongBao   { get; set; } = string.Empty;
}
