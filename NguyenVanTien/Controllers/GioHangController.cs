using NguyenVanTien.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NguyenVanTien.Controllers
{
    public class GioHangController : Controller
    {
        // Database context
        SachOnlineEntities3 db = new SachOnlineEntities3();

        // Lấy giỏ hàng từ Session
        public List<GioHang> LayGioHang()
        {
            List<GioHang> lstGioHang = Session["GioHang"] as List<GioHang>;
            if (lstGioHang == null)
            {
                lstGioHang = new List<GioHang>();
                Session["GioHang"] = lstGioHang;
            }
            return lstGioHang;
        }

        // Thêm sách vào giỏ hàng
        public ActionResult ThemGioHang(int id, string url)
        {
            List<GioHang> lstGioHang = LayGioHang();

            // Kiểm tra nếu sách đã có trong giỏ hàng
            GioHang item = lstGioHang.SingleOrDefault(s => s.iMaSach == id);
            if (item == null)
            {
                // Nếu chưa có, thêm sách mới vào giỏ hàng
                item = new GioHang(id);
                lstGioHang.Add(item);
            }
            else
            {
                // Nếu đã có, tăng số lượng
                item.iSoLuong++;
            }
            return Redirect(url); // Quay lại trang hiện tại
        }

        // Tính tổng số lượng sách trong giỏ hàng
        public int TongSoLuong()
        {
            List<GioHang> lstGioHang = LayGioHang();
            return lstGioHang.Sum(s => s.iSoLuong);
        }

        // Tính tổng tiền của giỏ hàng
        public double TongTien()
        {
            List<GioHang> lstGioHang = LayGioHang();
            return lstGioHang.Sum(s => s.dThanhTien);
        }

        // Move CartPartial() here
        public PartialViewResult CartPartial()
        {
            List<GioHang> lstGioHang = LayGioHang();
            ViewBag.TongSoLuong = lstGioHang.Sum(s => s.iSoLuong);
            return PartialView("CartPartial");  // This tells MVC to render the CartPartial view
        }
        // Hiển thị giỏ hàng
        public ActionResult GioHang()
        {
            List<GioHang> lstGioHang = LayGioHang();
            if (lstGioHang.Count == 0)
            {
                return RedirectToAction("Index", "Home"); // Nếu giỏ hàng trống, quay về trang chủ
            }

            // Lưu thông tin tổng số lượng và tổng tiền vào ViewBag để hiển thị trong View
            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TongTien = TongTien();
            return View(lstGioHang);
        }

        //Cập nhật giỏ hàng
        [HttpPost]
        public ActionResult CapNhatGioHang(int iMaSach, int SoLuongMoi)
        {
            // Get the cart from session
            List<GioHang> lstGioHang = LayGioHang();

            // Find the item in the cart
            GioHang item = lstGioHang.SingleOrDefault(s => s.iMaSach == iMaSach);

            if (item != null)
            {
                // Update the quantity
                item.iSoLuong = SoLuongMoi;
            }

            // Redirect to the cart page to show the updated cart
            return RedirectToAction("GioHang");
        }

        // xoá giỏ hàng
        public ActionResult XoaGioHang(int iMaSach)
        {
            // Get the cart from session
            List<GioHang> lstGioHang = LayGioHang();

            // Find the item in the cart
            GioHang item = lstGioHang.SingleOrDefault(s => s.iMaSach == iMaSach);

            if (item != null)
            {
                // Remove the item from the cart
                lstGioHang.Remove(item);
            }

            // Redirect to the cart page
            return RedirectToAction("GioHang");
        }

        // xoá sạch
        // xoá giỏ hàng
        // Xóa toàn bộ giỏ hàng
        public ActionResult XoaHet()
        {
            // Get the cart from session
            List<GioHang> lstGioHang = LayGioHang();

            // Clear the cart
            lstGioHang.Clear();

            // Redirect to the cart page
            return RedirectToAction("GioHang");
        }

    }
}
