using Microsoft.EntityFrameworkCore;
using QLDatVeXe.Domain.Business;
using QLDatVeXe.Domain.Constants;
using QLDatVeXe.Domain.DTOs;
using QLDatVeXe.Models;
using QLDatVeXe.Repositories.Interfaces;

namespace QLDatVeXe.Repositories.Implementations;

public class DonDatVeRepository : IDonDatVeRepository
{
    private readonly QldatVeXeContext _db;
    public DonDatVeRepository(QldatVeXeContext db) => _db = db;

    public async Task<DatVeResultDTO> DatVeAsync(DatVeDTO dto, string maKH)
    {
        await using var transaction = await _db.Database.BeginTransactionAsync();
        try
        {
            // 1. Kiểm tra ghế còn trống
            var gheDaDat = await _db.Chitietdatve
                .Where(ct => ct.MaChuyen == dto.MaChuyen && ct.TrangThaiVe != TrangThaiDon.DaHuy
                          && dto.DsGhe.Contains(ct.MaGhe))
                .Select(ct => ct.MaGhe)
                .ToListAsync();
            if (gheDaDat.Any())
                return new DatVeResultDTO { ThanhCong = false, ThongBao = "Một số ghế đã được đặt bởi người khác. Vui lòng chọn lại." };

            // 2. Lấy giá vé
            var chuyen = await _db.Chuyenxe.FindAsync(dto.MaChuyen);
            if (chuyen is null)
                return new DatVeResultDTO { ThanhCong = false, ThongBao = "Không tìm thấy chuyến xe." };

            // 3. Khuyến mãi
            Khuyenmai? km = null;
            if (!string.IsNullOrEmpty(dto.MaKM))
            {
                km = await _db.Khuyenmai.FirstOrDefaultAsync(k => k.TenKhuyenMai == dto.MaKM
                                                                && k.NgayBatDau <= DateTime.Now
                                                                && k.NgayKetThuc >= DateTime.Now);
                if (km != null)
                {
                    var isUsed = await _db.DondatveKhuyenmai.AnyAsync(dk => dk.MaKm == km.MaKm && dk.MaDonNavigation.MaKh == maKH);
                    if (isUsed) km = null;
                }
            }

            decimal tongTien = DatVeBusiness.TinhTongTien(chuyen.GiaVe, dto.DsGhe.Count, km);

            // 4. Tạo DONDATVE
            var maDon = DatVeBusiness.SinhMaDon();
            var don = new Dondatve
            {
                MaDon       = maDon,
                MaKh        = maKH,
                NgayDat     = DateTime.Now,
                TongTien    = tongTien,
                TienCoc     = tongTien, // đặt cọc toàn bộ
                TrangThai   = TrangThaiDon.ChoXuLy,
                Tennguoidi  = dto.TenNguoiDi,
                Sdtnguoidi  = dto.SdtNguoiDi,
                GhiChu      = dto.GhiChu
            };
            _db.Dondatve.Add(don);
            await _db.SaveChangesAsync();

            // 5. Tạo CHITIETDATVE (mỗi ghế 1 record)
            foreach (var maGhe in dto.DsGhe)
            {
                _db.Chitietdatve.Add(new Chitietdatve
                {
                    MaCtdat     = DatVeBusiness.SinhMaCtDat(),
                    MaDon       = maDon,
                    MaChuyen    = dto.MaChuyen,
                    MaGhe       = maGhe,
                    GiaVeLucDat = chuyen.GiaVe,
                    TrangThaiVe = TrangThaiVe.DaDat
                });
            }
            await _db.SaveChangesAsync();

            // 6. Áp dụng khuyến mãi
            if (km is not null)
            {
                decimal soTienGiam = chuyen.GiaVe * dto.DsGhe.Count - tongTien;
                _db.DondatveKhuyenmai.Add(new DondatveKhuyenmai
                {
                    MaDon      = maDon,
                    MaKm       = km.MaKm,
                    SoTienGiam = soTienGiam
                });
                await _db.SaveChangesAsync();
            }

            // 7. Tạo THANHTOAN
            _db.Thanhtoan.Add(new Thanhtoan
            {
                MaTt       = DatVeBusiness.SinhMaTT(),
                MaDon      = maDon,
                SoTien     = tongTien,
                PhuongThuc = dto.PhuongThuc,
                ThoiGianTt = DateTime.Now,
                TrangThai  = TrangThaiThanhToan.ThanhCong
            });
            await _db.SaveChangesAsync();

            await transaction.CommitAsync();
            return new DatVeResultDTO { ThanhCong = true, MaDon = maDon, ThongBao = "Đặt vé thành công!" };
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return new DatVeResultDTO { ThanhCong = false, ThongBao = "Lỗi hệ thống: " + ex.Message };
        }
    }

    public async Task<List<Dondatve>> GetLichSuAsync(string maKH)
        => await _db.Dondatve
            .Include(d => d.Chitietdatve)
                .ThenInclude(ct => ct.MaChuyenNavigation)
                    .ThenInclude(cx => cx.MaTuyenNavigation)
                        .ThenInclude(t => t.MaBenDiNavigation)
                            .ThenInclude(b => b.MaPhuongNoNavigation)
                                .ThenInclude(p => p.MaTinhNoNavigation)
            .Include(d => d.Chitietdatve)
                .ThenInclude(ct => ct.MaChuyenNavigation)
                    .ThenInclude(cx => cx.MaTuyenNavigation)
                        .ThenInclude(t => t.MaBenDenNavigation)
                            .ThenInclude(b => b.MaPhuongNoNavigation)
                                .ThenInclude(p => p.MaTinhNoNavigation)
            .Include(d => d.Thanhtoan)
            .Where(d => d.MaKh == maKH)
            .OrderByDescending(d => d.NgayDat)
            .ToListAsync();

    public async Task<bool> HuyDonAsync(string maDon, string maKH)
    {
        var don = await _db.Dondatve
            .FirstOrDefaultAsync(d => d.MaDon == maDon && d.MaKh == maKH);
        if (don is null || don.TrangThai != TrangThaiDon.ChoXuLy) return false;
        don.TrangThai = TrangThaiDon.DaHuy;
        // Hủy chi tiết
        var ctList = await _db.Chitietdatve.Where(ct => ct.MaDon == maDon).ToListAsync();
        foreach (var ct in ctList) ct.TrangThaiVe = TrangThaiVe.DaHuy;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<List<Dondatve>> GetAllAsync(string? trangThai = null)
    {
        var q = _db.Dondatve
            .Include(d => d.MaKhNavigation)
            .Include(d => d.Chitietdatve)
            .AsQueryable();
        if (!string.IsNullOrEmpty(trangThai))
            q = q.Where(d => d.TrangThai == trangThai);
        return await q.OrderByDescending(d => d.NgayDat).ToListAsync();
    }

    public async Task<Dondatve?> GetByIdAsync(string maDon)
        => await _db.Dondatve
            .Include(d => d.MaKhNavigation)
            .Include(d => d.Chitietdatve)
                .ThenInclude(ct => ct.MaGheNavigation)
            .Include(d => d.Chitietdatve)
                .ThenInclude(ct => ct.MaChuyenNavigation)
            .Include(d => d.Thanhtoan)
            .FirstOrDefaultAsync(d => d.MaDon == maDon);

    public async Task DuyetDonAsync(string maDon)
    {
        var don = await _db.Dondatve.FindAsync(maDon);
        if (don is not null)
        {
            don.TrangThai = TrangThaiDon.ThanhCong;
            await _db.SaveChangesAsync();
        }
    }

    public async Task HuyDonAdminAsync(string maDon)
    {
        var don = await _db.Dondatve.FindAsync(maDon);
        if (don is not null)
        {
            don.TrangThai = TrangThaiDon.DaHuy;
            var ctList = await _db.Chitietdatve.Where(ct => ct.MaDon == maDon).ToListAsync();
            foreach (var ct in ctList) ct.TrangThaiVe = TrangThaiVe.DaHuy;
            await _db.SaveChangesAsync();
        }
    }

    public async Task<Khuyenmai?> GetKhuyenMaiByTenAsync(string tenKM)
        => await _db.Khuyenmai.FirstOrDefaultAsync(k => k.TenKhuyenMai == tenKM
                                                    && k.NgayBatDau <= DateTime.Now
                                                    && k.NgayKetThuc >= DateTime.Now);

    public async Task<List<Khuyenmai>> GetActiveKhuyenMaiAsync()
        => await _db.Khuyenmai
            .Where(k => k.NgayBatDau <= DateTime.Now && k.NgayKetThuc >= DateTime.Now)
            .ToListAsync();

    public async Task<List<Khuyenmai>> GetUnusedKhuyenMaiAsync(string maKH)
    {
        var usedKmIds = await _db.DondatveKhuyenmai
            .Include(dk => dk.MaDonNavigation)
            .Where(dk => dk.MaDonNavigation.MaKh == maKH)
            .Select(dk => dk.MaKm)
            .ToListAsync();

        return await _db.Khuyenmai
            .Where(k => k.NgayBatDau <= DateTime.Now && k.NgayKetThuc >= DateTime.Now && !usedKmIds.Contains(k.MaKm))
            .ToListAsync();
    }
}
