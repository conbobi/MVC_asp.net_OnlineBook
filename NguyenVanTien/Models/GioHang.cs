using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NguyenVanTien.Models
{
    public class GioHang
    {
        SachOnlineEntities3 db=new SachOnlineEntities3();

        public int iMaSach { get; set; }
        public string sTenSach { get; set; }
        public string sAnhBia { get; set; }
        public int iSoLuong { get; set; }
        public double dDonGia { get; set; }
        public double dThanhTien
        {
            get
            {
                return iSoLuong * dDonGia;
            }
        }

        // Constructor to initialize the cart item based on the book ID
        public GioHang(int MaSach)
        {
            // Assume we have a data context to retrieve the book details

           
                iMaSach = MaSach;
            var sach = db.SACHes.Single(s => s.MaSach == MaSach);
            sTenSach = sach.TenSach;
                sAnhBia = sach.AnhBia;
                dDonGia =double.Parse( sach.GiaBan.ToString());
                iSoLuong = 1;
            
        }
    }

}