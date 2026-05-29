using Microsoft.AspNetCore.Mvc;
using QLDatVeXe.Helpers;
using QLDatVeXe.Repositories.Interfaces;

namespace QLDatVeXe.Controllers;

public class HomeController : Controller
{
    private readonly IChuyenXeRepository _chuyenXeRepo;

    public HomeController(IChuyenXeRepository chuyenXeRepo)
    {
        _chuyenXeRepo = chuyenXeRepo;
    }

    public async Task<IActionResult> Index()
    {
        var user         = SessionHelper.GetCurrentUser(HttpContext.Session);
        ViewBag.User     = user;
        ViewBag.DsTinhThanh = await _chuyenXeRepo.GetAllTinhThanhAsync();
        ViewBag.Upcoming = await _chuyenXeRepo.GetUpcomingAsync(6);
        ViewBag.TopMaTinh= await _chuyenXeRepo.GetTopDestinationsAsync(4);
        return View();
    }

    public IActionResult Error()
    {
        return View();
    }
}
