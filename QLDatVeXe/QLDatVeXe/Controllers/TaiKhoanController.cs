using Microsoft.AspNetCore.Mvc;

namespace QLDatVeXe.Controllers
{
    public class TaiKhoanController : Controller
    {
        // GET: /TaiKhoan/Login  hoặc  /Login
        [HttpGet]
        [Route("Login")]
        [Route("TaiKhoan/Login")]
        public IActionResult Login()
        {
            return View("~/Views/TaiKhoan/Login.cshtml");
        }

        // POST: /Login
        [HttpPost]
        [Route("Login")]
        public IActionResult LoginPost(string tenDangNhap, string matKhau)
        {
            // TODO: xử lý đăng nhập thực tế
            return RedirectToAction("Index", "Home");
        }

        // GET: /Register
        [HttpGet]
        [Route("Register")]
        [Route("TaiKhoan/Register")]
        public IActionResult Register()
        {
            return View("~/Views/TaiKhoan/Register.cshtml");
        }

        // POST: /Register
        [HttpPost]
        [Route("Register")]
        public IActionResult RegisterPost(string hoTen, string sdt, string tenDangNhap, string matKhau)
        {
            // TODO: xử lý đăng ký thực tế
            return RedirectToAction("Index", "Home");
        }

        // GET: /Account
        [HttpGet]
        [Route("Account")]
        [Route("TaiKhoan/Account")]
        public IActionResult Account(string? tab)
        {
            ViewData["Tab"] = tab ?? "orders";
            return View("~/Views/TaiKhoan/Account.cshtml");
        }

        // POST: /Account
        [HttpPost]
        [Route("Account")]
        public IActionResult AccountPost()
        {
            // TODO: xử lý cập nhật profile
            return RedirectToAction("Account");
        }

        // GET: /TicketDetail
        [HttpGet]
        [Route("TicketDetail")]
        [Route("TaiKhoan/TicketDetail")]
        public IActionResult TicketDetail(string? maDon)
        {
            return View("~/Views/TaiKhoan/TicketDetail.cshtml");
        }

        // POST: /TicketDetail (hủy vé, đánh giá)
        [HttpPost]
        [Route("TicketDetail")]
        public IActionResult TicketDetailPost(string action, string maDon)
        {
            // TODO: xử lý hủy vé / đánh giá
            return RedirectToAction("Account", new { tab = "orders" });
        }

        // GET: /Logout
        [Route("Logout")]
        public IActionResult Logout()
        {
            return RedirectToAction("Login");
        }
    }
}
