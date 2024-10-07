using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NguyenVanTien.Models;
namespace NguyenVanTien.Controllers
{
    public class NguyenVanTienController : Controller
    {
        SachOnlineEntities3 data = new SachOnlineEntities3();
        private List<SACH> LaySachMoi(int count)
        {
            return data.SACHes.OrderByDescending(a => a.NgayCapNhat).Take(count).ToList();
        }
        // GET: SachOnline
        
        public ActionResult Index()
        {
            var listSachMoi = LaySachMoi(6);
            return View(listSachMoi);
        }
        public ActionResult ChuDePartial()
        {
            var listChuDe = from cd in data.CHUDEs select cd;
            return PartialView(listChuDe);
        }
        public ActionResult SliderPartial()
        {
            return PartialView();
        }
        public ActionResult NhaXuatBanPartial()
        {
            var listNXB = data.NHAXUATBANs.ToList();
            return PartialView(listNXB);
        }
        public ActionResult SachBanNhieuPartial()
        {
            return PartialView();
        }
        public ActionResult FooterPartial()
        {
            return PartialView();
        }
        public ActionResult NavPartial()
        {
            return PartialView();
        }



        public ActionResult SachTheoChuDe(int id)
        {
            // Lấy danh sách sách theo chủ đề
            var sachList = data.SACHes.Where(s => s.MaCD == id).ToList();

            // Lấy tên chủ đề từ bảng Chủ đề dựa trên id
            var chuDe = data.CHUDEs.SingleOrDefault(c => c.MaCD == id);

            // Truyền tên chủ đề vào ViewBag để hiển thị
            ViewBag.TenChuDe = chuDe != null ? chuDe.TenChuDe: "Chủ đề không tồn tại";

            return View(sachList);
        }



        public ActionResult SachTheoNXB(int id)
        {
            var sach = data.SACHes.Where(s => s.MaNXB == id).ToList();

            // Lấy tên Nhà Xuất Bản để hiển thị
            var nxb = data.NHAXUATBANs.SingleOrDefault(n => n.MaNXB == id);
            ViewBag.TenNXB = nxb != null ? nxb.TenNXB : "Nhà xuất bản không tồn tại";

            return View(sach);
        }
        public ActionResult ChiTietSach(int id)
        {
            // Lấy sách theo MaSach
            var sach = data.SACHes.SingleOrDefault(s => s.MaSach == id);

            // Kiểm tra nếu sách không tồn tại
            if (sach == null)
            {
                return HttpNotFound("Sách không tồn tại");
            }

            // Trả về View và truyền dữ liệu sách
            return View(sach);
        }

        
    }
}

