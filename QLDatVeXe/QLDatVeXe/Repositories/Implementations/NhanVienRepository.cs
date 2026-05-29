using Microsoft.EntityFrameworkCore;
using QLDatVeXe.Domain.Constants;
using QLDatVeXe.Models;
using QLDatVeXe.Repositories.Interfaces;

namespace QLDatVeXe.Repositories.Implementations;

public class NhanVienRepository : INhanVienRepository
{
    private readonly QldatVeXeContext _db;
    public NhanVienRepository(QldatVeXeContext db) => _db = db;

    public async Task<Nhanvien?> GetByTenDangNhapAsync(string tenDangNhap)
        => await _db.Nhanvien.FirstOrDefaultAsync(n => n.TenDangNhap == tenDangNhap);

    public async Task<Nhanvien?> GetByIdAsync(string maNV)
        => await _db.Nhanvien.FindAsync(maNV);

    public async Task<bool> TenDangNhapTonTaiAsync(string tenDangNhap)
        => await _db.Nhanvien.AnyAsync(n => n.TenDangNhap == tenDangNhap)
        || await _db.Khachhang.AnyAsync(k => k.TenDangNhap == tenDangNhap);

    public async Task<List<Nhanvien>> GetAllAsync()
        => await _db.Nhanvien
            .Include(n => n.MaPhuongNoNavigation)
                .ThenInclude(p => p!.MaTinhNoNavigation)
            .OrderBy(n => n.HoTen)
            .ToListAsync();

    public async Task<string> SinhMaNVAsync()
    {
        var count = await _db.Nhanvien.CountAsync();
        return $"NV{(count + 1):D3}";
    }

    public async Task<Nhanvien> TaoMoiAsync(Nhanvien nv)
    {
        if (string.IsNullOrEmpty(nv.MaNv))
            nv.MaNv = await SinhMaNVAsync();
        nv.NgayVaoLam = DateOnly.FromDateTime(DateTime.Today);
        nv.TrangThai  = TrangThaiNhanVien.DangLam;
        _db.Nhanvien.Add(nv);
        await _db.SaveChangesAsync();
        return nv;
    }

    public async Task UpdateAsync(Nhanvien nv)
    {
        _db.Nhanvien.Update(nv);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(string maNV)
    {
        var nv = await _db.Nhanvien.FindAsync(maNV);
        if (nv is not null)
        {
            nv.TrangThai = TrangThaiNhanVien.NghiViec;
            await _db.SaveChangesAsync();
        }
    }
}
