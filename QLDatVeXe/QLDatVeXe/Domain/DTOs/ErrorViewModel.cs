namespace QLDatVeXe.Domain.DTOs;

/// <summary>
/// ViewModel truyền thông tin lỗi xuống trang Error.cshtml.
/// Không phải Entity Database First — được đặt trong Domain/DTOs.
/// </summary>
public class ErrorViewModel
{
    public string? RequestId { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}
