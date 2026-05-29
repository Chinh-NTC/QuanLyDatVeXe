using QLDatVeXe.Domain.DTOs;
using QLDatVeXe.Models;

namespace QLDatVeXe.Repositories.Interfaces;

public interface IDonDatVeRepository
{
    /// <summary>Tạo đơn mới (transaction: DONDATVE + CHITIETDATVE + THANHTOAN)</summary>
    Task<DatVeResultDTO> DatVeAsync(DatVeDTO dto, string maKH);

    /// <summary>Lịch sử đơn của khách hàng</summary>
    Task<List<Dondatve>> GetLichSuAsync(string maKH);

    /// <summary>Hủy đơn (chỉ CHOXULY mới được hủy)</summary>
    Task<bool> HuyDonAsync(string maDon, string maKH);

    // ── Quản lý ─────────────────────────────────────────────────────────────
    Task<List<Dondatve>> GetAllAsync(string? trangThai = null);
    Task<Dondatve?> GetByIdAsync(string maDon);
    Task DuyetDonAsync(string maDon);
    Task HuyDonAdminAsync(string maDon);

    // ── Khuyến mãi ──────────────────────────────────────────────────────────
    Task<Khuyenmai?> GetKhuyenMaiByTenAsync(string tenKM);
    Task<List<Khuyenmai>> GetActiveKhuyenMaiAsync();
    Task<List<Khuyenmai>> GetUnusedKhuyenMaiAsync(string maKH);
}
