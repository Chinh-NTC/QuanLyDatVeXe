using QLDatVeXe.Domain.DTOs;
using QLDatVeXe.Models;

namespace QLDatVeXe.Repositories.Interfaces;

public interface IChuyenXeRepository
{
    /// <summary>Tìm chuyến theo tỉnh đi, tỉnh đến, ngày đi (dùng V_CHUYENXE)</summary>
    Task<List<VChuyenxe>> TimChuyenAsync(string? maTinhDi, string? maTinhDen, DateOnly? ngayDi);

    /// <summary>Chi tiết 1 chuyến từ view V_CHUYENXE</summary>
    Task<VChuyenxe?> GetChiTietAsync(string maChuyen);

    /// <summary>Danh sách ghế của xe trong chuyến, annotate trạng thái đã đặt</summary>
    Task<List<Ghe>> GetDanhSachGheAsync(string maChuyen);

    /// <summary>Chuyến sắp khởi hành (trang chủ)</summary>
    Task<List<VChuyenxe>> GetUpcomingAsync(int take = 5);

    /// <summary>Lấy top địa điểm đến phổ biến dựa trên chuyến sắp tới</summary>
    Task<List<string>> GetTopDestinationsAsync(int take = 4);

    // ── Quản lý (CRUD) ──────────────────────────────────────────────────────
    Task<List<Chuyenxe>> GetAllForAdminAsync();
    Task<Chuyenxe?> GetByIdAsync(string maChuyen);
    Task ThemChuyenAsync(Chuyenxe chuyen);
    Task SuaChuyenAsync(Chuyenxe chuyen);
    Task HuyChuyenAsync(string maChuyen);

    // ── Dữ liệu phụ ─────────────────────────────────────────────────────────
    Task<List<Tuyenduong>> GetAllTuyenAsync();
    Task<List<Xe>> GetAllXeAsync();
    Task<List<Tinhthanh>> GetAllTinhThanhAsync();
}
