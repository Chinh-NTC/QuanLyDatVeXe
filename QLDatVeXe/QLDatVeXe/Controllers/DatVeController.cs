using Microsoft.AspNetCore.Mvc;

namespace QLDatVeXe.Controllers
{
    public class DatVeController : Controller
    {
        // GET: /Search  (tìm chuyến)
        [HttpGet]
        [Route("Search")]
        [Route("DatVe/ChonChuyen")]
        public IActionResult ChonChuyen(string? maTinhDi, string? maTinhDen, string? ngayDi)
        {
            return View("~/Views/DatVe/ChonChuyen.cshtml");
        }

        // GET: /Booking  (chọn ghế)
        [HttpGet]
        [Route("Booking")]
        [Route("DatVe/ChonGhe")]
        public IActionResult ChonGhe(string? maChuyen)
        {
            return View("~/Views/DatVe/ChonGhe.cshtml");
        }

        // GET: /Payment  (thanh toán)
        [HttpGet]
        [Route("Payment")]
        [Route("DatVe/ThanhToan")]
        public IActionResult ThanhToan(string? maChuyen)
        {
            return View("~/Views/DatVe/ThanhToan.cshtml");
        }

        // POST: /Payment (xác nhận đặt vé)
        [HttpPost]
        [Route("Payment")]
        public IActionResult XacNhanDatVe(string maChuyen, string maGhes, string phuongThuc, string? promoCode)
        {
            // TODO: xử lý đặt vé thực tế
            return RedirectToAction("Account", "TaiKhoan", new { tab = "orders" });
        }
    }
}
