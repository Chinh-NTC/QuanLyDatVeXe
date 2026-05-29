using Microsoft.EntityFrameworkCore;
using QLDatVeXe.Domain.Constants;
using QLDatVeXe.Models;
using QLDatVeXe.Repositories.Interfaces;

namespace QLDatVeXe.Repositories.Implementations;

public class KhachHangRepository : IKhachHangRepository
{
    private readonly QldatVeXeContext _db;
    public KhachHangRepository(QldatVeXeContext db) => _db = db;

    public async Task<Khachhang?> GetByTenDangNhapAsync(string tenDangNhap)
        => await _db.Khachhang.FirstOrDefaultAsync(k => k.TenDangNhap == tenDangNhap);

    public async Task<Khachhang?> GetByIdAsync(string maKH)
        => await _db.Khachhang.FindAsync(maKH);

    public async Task<bool> TenDangNhapTonTaiAsync(string tenDangNhap)
        => await _db.Khachhang.AnyAsync(k => k.TenDangNhap == tenDangNhap)
        || await _db.Nhanvien.AnyAsync(n => n.TenDangNhap == tenDangNhap);

    public async Task<bool> SdtTonTaiAsync(string sdt)
        => await _db.Khachhang.AnyAsync(k => k.Sdt == sdt)
        || await _db.Nhanvien.AnyAsync(n => n.Sdt == sdt);

    public async Task<List<Khachhang>> GetAllAsync()
        => await _db.Khachhang.OrderBy(k => k.HoTen).ToListAsync();

    public async Task<Khachhang> TaoMoiAsync(Khachhang kh)
    {
        // Sinh maKH tự động an toàn
        var maxKH = await _db.Khachhang
                             .OrderByDescending(k => k.MaKh)
                             .FirstOrDefaultAsync();

        if (maxKH == null)
        {
            kh.MaKh = "KH01";
        }
        else
        {
            // Trích xuất phần số từ maxKH.MaKh (ví dụ: "KH08" -> 8, "KH0009" -> 9)
            string numberPart = maxKH.MaKh.Substring(2);
            if (int.TryParse(numberPart, out int currentId))
            {
                kh.MaKh = $"KH{(currentId + 1):D2}";
            }
            else
            {
                kh.MaKh = $"KH{new Random().Next(100, 9999)}";
            }
        }

        kh.NgayTao = DateTime.Now;
        _db.Khachhang.Add(kh);
        await _db.SaveChangesAsync();
        return kh;
    }

    public async Task UpdateAsync(Khachhang kh)
    {
        _db.Khachhang.Update(kh);
        await _db.SaveChangesAsync();
    }

    public async Task<List<Danhgia>> GetDanhGiaByKhachHangAsync(string maKH)
    {
        return await _db.Danhgia
            .Include(d => d.MaChuyenNavigation)
                .ThenInclude(c => c.MaTuyenNavigation)
                    .ThenInclude(t => t.MaBenDiNavigation)
            .Include(d => d.MaChuyenNavigation)
                .ThenInclude(c => c.MaTuyenNavigation)
                    .ThenInclude(t => t.MaBenDenNavigation)
            .Where(d => d.MaKh == maKH)
            .OrderByDescending(d => d.NgayDanhGia)
            .ToListAsync();
    }
}
