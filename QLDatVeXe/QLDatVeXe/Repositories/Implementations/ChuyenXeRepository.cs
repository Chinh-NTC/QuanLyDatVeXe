using Microsoft.EntityFrameworkCore;
using QLDatVeXe.Domain.Business;
using QLDatVeXe.Domain.Constants;
using QLDatVeXe.Domain.DTOs;
using QLDatVeXe.Models;
using QLDatVeXe.Repositories.Interfaces;

namespace QLDatVeXe.Repositories.Implementations;

public class ChuyenXeRepository : IChuyenXeRepository
{
    private readonly QldatVeXeContext _db;
    public ChuyenXeRepository(QldatVeXeContext db) => _db = db;

    public async Task<List<VChuyenxe>> TimChuyenAsync(string? maTinhDi, string? maTinhDen, DateOnly? ngayDi)
    {
        var q = _db.VChuyenxe.AsQueryable();
        if (!string.IsNullOrEmpty(maTinhDi))  q = q.Where(c => c.MaTinhDi  == maTinhDi);
        if (!string.IsNullOrEmpty(maTinhDen)) q = q.Where(c => c.MaTinhDen == maTinhDen);
        if (ngayDi.HasValue)                  q = q.Where(c => c.NgayDi    == ngayDi.Value);
        var result = await q.OrderBy(c => c.NgayDi).ThenBy(c => c.GioDi).ToListAsync();

        var now = DateTime.Now;
        foreach (var c in result)
        {
            if (c.TrangThaiChuyen != TrangThaiChuyen.Hoan && c.TrangThaiChuyen != TrangThaiChuyen.Huy)
            {
                var departureTime = c.NgayDi.ToDateTime(c.GioDi);
                var arrivalTime = departureTime.AddMinutes(c.ThoiGianDuKien);

                if (now >= arrivalTime)
                    c.TrangThaiChuyen = TrangThaiChuyen.HoanThanh;
                else if (now >= departureTime)
                    c.TrangThaiChuyen = TrangThaiChuyen.DangDi;
                else
                    c.TrangThaiChuyen = TrangThaiChuyen.SapDi;
            }
        }

        return result;
    }

    public async Task<VChuyenxe?> GetChiTietAsync(string maChuyen)
        => await _db.VChuyenxe.FirstOrDefaultAsync(c => c.MaChuyen == maChuyen);

    public async Task<List<Ghe>> GetDanhSachGheAsync(string maChuyen)
    {
        // Lấy bienSo từ CHUYENXE, sau đó lấy tất cả ghế của xe đó
        var chuyen = await _db.Chuyenxe.FindAsync(maChuyen);
        if (chuyen is null) return new List<Ghe>();

        // Lấy danh sách maGhe đã đặt trong chuyến này
        var gheDaDat = await _db.Chitietdatve
            .Where(ct => ct.MaChuyen == maChuyen && ct.TrangThaiVe != TrangThaiVe.DaHuy)
            .Select(ct => ct.MaGhe)
            .ToHashSetAsync();

        var dsGhe = await _db.Ghe
            .Where(g => g.BienSo == chuyen.BienSo && g.TrangThai != "HONG")
            .OrderBy(g => g.Tang).ThenBy(g => g.SoGhe)
            .ToListAsync();

        // Annotate: nếu đã đặt thì đổi trạng thái sang "DADAT" để view hiển thị
        foreach (var g in dsGhe)
        {
            if (gheDaDat.Contains(g.MaGhe))
                g.TrangThai = TrangThaiVe.DaDat;
        }
        return dsGhe;
    }

    public async Task<List<VChuyenxe>> GetUpcomingAsync(int take = 5)
    {
        var now = DateTime.Now;
        var today = DateOnly.FromDateTime(now);
        var currentTime = TimeOnly.FromDateTime(now);

        var query = await _db.VChuyenxe
            .Where(c => c.NgayDi >= today && c.TrangThaiChuyen == TrangThaiChuyen.SapDi && c.SoGheTrong > 0)
            .OrderBy(c => c.NgayDi).ThenBy(c => c.GioDi)
            .ToListAsync();

        return query
            .Where(c => c.NgayDi > today || (c.NgayDi == today && c.GioDi > currentTime))
            .Take(take)
            .ToList();
    }

    public async Task<List<string>> GetTopDestinationsAsync(int take = 4)
    {
        var now = DateTime.Now;
        var today = DateOnly.FromDateTime(now);
        var currentTime = TimeOnly.FromDateTime(now);

        var upcomingTrips = await _db.VChuyenxe
            .Where(c => c.NgayDi >= today && c.TrangThaiChuyen == TrangThaiChuyen.SapDi)
            .ToListAsync();

        var topMaTinh = upcomingTrips
            .Where(c => c.NgayDi > today || (c.NgayDi == today && c.GioDi > currentTime))
            .Where(c => !string.IsNullOrEmpty(c.MaTinhDen))
            .GroupBy(c => c.MaTinhDen)
            .OrderByDescending(g => g.Count())
            .Select(g => g.Key!)
            .Take(take)
            .ToList();

        return topMaTinh;
    }

    public async Task<List<Chuyenxe>> GetAllForAdminAsync()
        => await _db.Chuyenxe
            .Include(c => c.MaTuyenNavigation)
                .ThenInclude(t => t.MaBenDiNavigation)
            .Include(c => c.MaTuyenNavigation)
                .ThenInclude(t => t.MaBenDenNavigation)
            .Include(c => c.BienSoNavigation)
                .ThenInclude(x => x.MaNhaXeNavigation)
            .OrderByDescending(c => c.NgayDi)
            .ToListAsync();

    public async Task<Chuyenxe?> GetByIdAsync(string maChuyen)
        => await _db.Chuyenxe
            .Include(c => c.MaTuyenNavigation)
                .ThenInclude(t => t.MaBenDiNavigation)
            .Include(c => c.MaTuyenNavigation)
                .ThenInclude(t => t.MaBenDenNavigation)
            .Include(c => c.BienSoNavigation)
            .FirstOrDefaultAsync(c => c.MaChuyen == maChuyen);

    public async Task ThemChuyenAsync(Chuyenxe chuyen)
    {
        if (string.IsNullOrEmpty(chuyen.MaChuyen))
        {
            var count = await _db.Chuyenxe.CountAsync();
            chuyen.MaChuyen = $"CX{(count + 1):D5}";
        }
        chuyen.TrangThai ??= TrangThaiChuyen.SapDi;
        _db.Chuyenxe.Add(chuyen);
        await _db.SaveChangesAsync();
    }

    public async Task SuaChuyenAsync(Chuyenxe chuyen)
    {
        _db.Chuyenxe.Update(chuyen);
        await _db.SaveChangesAsync();
    }

    public async Task HuyChuyenAsync(string maChuyen)
    {
        var cx = await _db.Chuyenxe.FindAsync(maChuyen);
        if (cx is not null)
        {
            cx.TrangThai = TrangThaiChuyen.Huy;
            await _db.SaveChangesAsync();
        }
    }

    public async Task<List<Tuyenduong>> GetAllTuyenAsync()
        => await _db.Tuyenduong
            .Include(t => t.MaBenDiNavigation)
            .Include(t => t.MaBenDenNavigation)
            .ToListAsync();

    public async Task<List<Xe>> GetAllXeAsync()
        => await _db.Xe
            .Include(x => x.MaNhaXeNavigation)
            .Where(x => x.TrangThai == "SANSANG")
            .ToListAsync();

    public async Task<List<Tinhthanh>> GetAllTinhThanhAsync()
        => await _db.Tinhthanh.OrderBy(t => t.TenTinh).ToListAsync();
}
