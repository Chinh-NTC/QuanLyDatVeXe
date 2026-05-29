using QLDatVeXe.Models;

namespace QLDatVeXe.Repositories.Interfaces;

public interface INhanVienRepository
{
    Task<Nhanvien?> GetByTenDangNhapAsync(string tenDangNhap);
    Task<Nhanvien?> GetByIdAsync(string maNV);
    Task<bool>      TenDangNhapTonTaiAsync(string tenDangNhap);
    Task<List<Nhanvien>> GetAllAsync();
    Task<Nhanvien>  TaoMoiAsync(Nhanvien nv);
    Task UpdateAsync(Nhanvien nv);
    Task DeleteAsync(string maNV);
    Task<string> SinhMaNVAsync();
}
