using QLDatVeXe.Models;

namespace QLDatVeXe.Repositories.Interfaces;

public interface IKhachHangRepository
{
    Task<Khachhang?> GetByTenDangNhapAsync(string tenDangNhap);
    Task<Khachhang?> GetByIdAsync(string maKH);
    Task<bool>       TenDangNhapTonTaiAsync(string tenDangNhap);
    Task<bool>       SdtTonTaiAsync(string sdt);
    Task<Khachhang>  TaoMoiAsync(Khachhang kh);
    Task<List<Khachhang>> GetAllAsync();
    Task UpdateAsync(Khachhang kh);
    Task<List<Danhgia>> GetDanhGiaByKhachHangAsync(string maKH);
}
