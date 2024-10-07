using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web.Mvc;
using System.Web.Security;
using NguyenVanTien.Models; // Thay đổi namespace nếu cần

public class UserController : Controller
{
    SachOnlineEntities3 data = new SachOnlineEntities3();

    [HttpGet]
    public ActionResult DangKy()
    {
        return View();
    }

    [HttpPost]
    public ActionResult DangKy(KHACHHANG kh, string MatKhauNL)
    {
        // Kiểm tra tên đăng nhập đã tồn tại hay chưa
        var existingUserByUsername = data.KHACHHANGs.FirstOrDefault(u => u.TaiKhoan == kh.TaiKhoan);
        if (existingUserByUsername != null)
        {
            ModelState.AddModelError("TaiKhoan", "Tên đăng nhập đã tồn tại.");
            return View();
        }

        // Kiểm tra email đã tồn tại hay chưa
        var existingUserByEmail = data.KHACHHANGs.FirstOrDefault(u => u.Email == kh.Email);
        if (existingUserByEmail != null)
        {
            ModelState.AddModelError("Email", "Email đã được sử dụng.");
            return View();
        }

        // Kiểm tra mật khẩu nhập lại có khớp hay không
        if (kh.MatKhau != MatKhauNL)
        {
            ModelState.AddModelError("MatKhauNL", "Mật khẩu nhập lại không khớp.");
            return View();
        }

        // Lưu người dùng mới vào cơ sở dữ liệu nếu không có lỗi
        if (ModelState.IsValid)
        {
            data.KHACHHANGs.Add(kh);
            data.SaveChanges();

            // Gửi email xác nhận
            GuiEmailXacNhan(kh.Email, kh);

            TempData["ThongBao"] = "Đăng ký thành công! Vui lòng kiểm tra email để xác nhận.";

            return RedirectToAction("DangNhap");
        }

        return View();
    }

    [HttpGet]
    public ActionResult DangNhap()
    {
        return View();
    }

    [HttpPost]
    public ActionResult DangNhap(string TenDN, string MatKhau)
    {
        // Kiểm tra người dùng trong cơ sở dữ liệu
        var user = data.KHACHHANGs.FirstOrDefault(u => u.TaiKhoan == TenDN && u.MatKhau == MatKhau);
        if (user != null)
        {
            // Nếu người dùng hợp lệ, lưu thông tin vào session
            Session["User"] = user; // Lưu thông tin người dùng vào session
            FormsAuthentication.SetAuthCookie(user.TaiKhoan, false); // Thiết lập cookie phiên làm việc

            return RedirectToAction("Index", "NguyenVanTien"); // Chuyển hướng đến trang chủ hoặc trang khác
        }
        else
        {
            ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng.");
        }

        return View();
    }

    public ActionResult DangXuat()
    {
        Session["User"] = null; // Xóa thông tin người dùng khỏi session
        FormsAuthentication.SignOut(); // Đăng xuất
        return RedirectToAction("Index", "NguyenVanTien"); // Chuyển hướng về trang chủ
    }

    // Hàm gửi email xác nhận
    private void GuiEmailXacNhan(string emailNguoiDung, KHACHHANG kh)
    {
        try
        {
            var fromAddress = new MailAddress("2224802010897@student.tdmu.edu.vn", "Web Sách Online");
            var toAddress = new MailAddress(emailNguoiDung);
            const string fromPassword = "vltk fwlq imbc ggrs";
            const string subject = "Xác nhận đăng ký thành công";

            // Tạo nội dung email với thông tin người dùng
            string body = $"Cảm ơn {kh.HoTen} đã đăng ký thành viên tại Web Sách Online!\n\n" +
                          $"Thông tin đăng kí của bạn bao gồm\n" +
                          $"Tên đăng nhập: {kh.TaiKhoan}\n" +
                          $"Số điện thoại: {kh.DienThoai}\n" +
                          $"email: {kh.Email}\n" +
                          $"Diachi: {kh.DiaChi}\n" +
                          "Chúng tôi sẽ liên lạc với bạn trong thời gian sớm nhất.";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };

            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            })
            {
                smtp.Send(message);
            }
        }
        catch (Exception ex)
        {
            ViewBag.ThongBao = "Có lỗi khi gửi email xác nhận: " + ex.Message;
        }

    }

    // --------------------------------------------------------bình luận-----------------------------------------------------------------------------


    public ActionResult BinhLuan(int maSach)
    {
        // Lấy tất cả bình luận theo MaSach
        var BinhLuans = data.BinhLuans.Where(b => b.MaSach == maSach).ToList();
        ViewBag.MaSach = maSach; // Gửi MaSach qua View để sử dụng khi thêm mới
        return View(BinhLuans); // Trả về danh sách bình luận
    }


    // thêm bình luận
    [HttpGet]
    public ActionResult ThemBinhLuan(int maSach)
    {
        ViewBag.MaSach = maSach;
        return View();
    }
    [HttpPost]
    public ActionResult ThemBinhLuan(int maSach, string NoiDung)
    {
        if (Session["User"] == null)
        {
            return RedirectToAction("DangNhap", "User");
        }

        var user = (KHACHHANG)Session["User"];
        var binhLuan = new BinhLuan
        {
            MaSach = maSach,
            NoiDung = NoiDung,
            TenNguoiBinhLuan = user.HoTen,
            NgayBinhLuan = DateTime.Now
        };

        try
        {
            data.BinhLuans.Add(binhLuan);
            data.SaveChanges(); // Lưu thay đổi
        }
        catch (Exception ex)
        {
            // Log hoặc xử lý lỗi tại đây
            ViewBag.Error = "Có lỗi khi lưu bình luận: " + ex.Message;
            return View(binhLuan);
        }

        return RedirectToAction("ChiTietSach", "NguyenVanTien", new { id = maSach });
    }

    // Sửa bình luận


    [HttpGet]
    public ActionResult SuaBinhLuan(int id)
    {
        var BinhLuan = data.BinhLuans.FirstOrDefault(b => b.MaBinhLuan == id);
        if (BinhLuan == null)
        {
            return HttpNotFound(); // Kiểm tra bình luận có tồn tại hay không
        }
        return View(BinhLuan); // Trả về view chỉnh sửa bình luận
    }

    [HttpPost]
    public ActionResult SuaBinhLuan(BinhLuan binhLuan)
    {
        if (ModelState.IsValid)
        {
            binhLuan.NgayBinhLuan = DateTime.Now; // Cập nhật ngày bình luận
            data.Entry(binhLuan).State = System.Data.Entity.EntityState.Modified; // Đánh dấu đã chỉnh sửa
            data.SaveChanges(); // Lưu thay đổi

            return RedirectToAction("ChiTietSach", "NguyenVanTien", new { id = binhLuan.MaSach }); // Chuyển hướng về chi tiết sách
        }
        return View(binhLuan);
    }
    // Xóa bình luận
    public ActionResult XoaBinhLuan(int id)
    {
        var BinhLuan = data.BinhLuans.FirstOrDefault(b => b.MaBinhLuan == id);
        if (BinhLuan == null)
        {
            return HttpNotFound();
        }
        data.BinhLuans.Remove(BinhLuan); // Xóa bình luận khỏi cơ sở dữ liệu
        data.SaveChanges(); // Lưu thay đổi
        return RedirectToAction("BinhLuan", new { maSach = BinhLuan.MaSach });
    }
}
