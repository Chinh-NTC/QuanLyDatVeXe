using Microsoft.EntityFrameworkCore;
using QLDatVeXe.Models;
using QLDatVeXe.Repositories.Implementations;
using QLDatVeXe.Repositories.Interfaces;

namespace QLDatVeXe.Extensions;

/// <summary>
/// Extension methods để đăng ký các service của ứng dụng vào DI container.
/// Tách khỏi Program.cs để giữ startup gọn gàng.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Đăng ký DbContext với connection string từ configuration.
    /// </summary>
    public static IServiceCollection AddDatabase(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<QldatVeXeContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
        return services;
    }

    /// <summary>
    /// Đăng ký tất cả Repository implementations.
    /// </summary>
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IKhachHangRepository, KhachHangRepository>();
        services.AddScoped<INhanVienRepository,  NhanVienRepository>();
        services.AddScoped<INhaxeRepository,     NhaxeRepository>();
        services.AddScoped<IChuyenXeRepository,  ChuyenXeRepository>();
        services.AddScoped<IDonDatVeRepository,  DonDatVeRepository>();
        return services;
    }

    /// <summary>
    /// Cấu hình Session cho ứng dụng.
    /// </summary>
    public static IServiceCollection AddAppSession(this IServiceCollection services)
    {
        services.AddDistributedMemoryCache();
        services.AddSession(options =>
        {
            options.IdleTimeout        = TimeSpan.FromHours(2);
            options.Cookie.HttpOnly    = true;
            options.Cookie.IsEssential = true;
            options.Cookie.Name        = ".QLDatVeXe.Session";
        });
        return services;
    }
}
