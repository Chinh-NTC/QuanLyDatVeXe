using QLDatVeXe.Models;

namespace QLDatVeXe.Repositories.Interfaces;

public interface INhaxeRepository
{
    Task<List<Nhaxe>> GetAllAsync();
    Task<Nhaxe?> GetByIdAsync(string maNhaXe);
    Task<Nhaxe?> GetNhaXeWithXeAsync(string maNhaXe);
    Task<List<Danhgia>> GetDanhGiaByNhaXeAsync(string maNhaXe);
    Task<Xe?> GetXeWithDanhGiaAsync(string bienSo);
}
