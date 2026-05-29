using Microsoft.EntityFrameworkCore;
using QLDatVeXe.Models;
using QLDatVeXe.Repositories.Interfaces;

namespace QLDatVeXe.Repositories.Implementations;

public class NhaxeRepository : INhaxeRepository
{
    private readonly QldatVeXeContext _db;
    public NhaxeRepository(QldatVeXeContext db) => _db = db;

    public async Task<List<Nhaxe>> GetAllAsync()
        => await _db.Nhaxe.Include(n => n.Xe).OrderBy(n => n.TenNhaXe).ToListAsync();

    public async Task<Nhaxe?> GetByIdAsync(string maNhaXe)
        => await _db.Nhaxe.FindAsync(maNhaXe);

    public async Task<Nhaxe?> GetNhaXeWithXeAsync(string maNhaXe)
    {
        return await _db.Nhaxe
            .Include(n => n.Xe)
            .FirstOrDefaultAsync(n => n.MaNhaXe == maNhaXe);
    }

    public async Task<List<Danhgia>> GetDanhGiaByNhaXeAsync(string maNhaXe)
    {
        return await _db.Danhgia
            .Include(d => d.MaKhNavigation)
            .Include(d => d.MaChuyenNavigation)
            .ThenInclude(c => c.BienSoNavigation)
            .Where(d => d.MaChuyenNavigation.BienSoNavigation.MaNhaXe == maNhaXe)
            .OrderByDescending(d => d.NgayDanhGia)
            .ToListAsync();
    }

    public async Task<Xe?> GetXeWithDanhGiaAsync(string bienSo)
    {
        return await _db.Xe
            .Include(x => x.MaNhaXeNavigation)
            .Include(x => x.Chuyenxe)
                .ThenInclude(c => c.Danhgia)
                    .ThenInclude(d => d.MaKhNavigation)
            .Include(x => x.Chuyenxe)
                .ThenInclude(c => c.MaTuyenNavigation)
                    .ThenInclude(t => t.MaBenDiNavigation)
                        .ThenInclude(b => b!.MaPhuongNoNavigation)
                            .ThenInclude(p => p!.MaTinhNoNavigation)
            .Include(x => x.Chuyenxe)
                .ThenInclude(c => c.MaTuyenNavigation)
                    .ThenInclude(t => t.MaBenDenNavigation)
                        .ThenInclude(b => b!.MaPhuongNoNavigation)
                            .ThenInclude(p => p!.MaTinhNoNavigation)
            .FirstOrDefaultAsync(x => x.BienSo == bienSo);
    }
}
