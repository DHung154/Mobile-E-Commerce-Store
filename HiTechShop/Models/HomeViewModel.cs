using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HiTechShop.Models
{
    public class HomeViewModel
    {
        public List<SanPham> DsSanPham { get; set; } = new List<SanPham>();
        public List<DanhMuc> DSDanhMuc { get; set; } = new List<DanhMuc>();
        public List<LoaiSP> DsLoaiSP { get; set; } = new List<LoaiSP>();
        public List<SanPham> HotProducts { get; set; } = new List<SanPham>();
        public List<SanPham> SaleProducts { get; set; } = new List<SanPham>();
        public List<SanPham> NewProducts { get; set; } = new List<SanPham>();
        public List<ThuongHieu> ThuongHieuList { get; set; } = new List<ThuongHieu>();
        public List<SanPham> SanPhamTheoTH { get; set; } = new List<SanPham>();
        public ThuongHieu ThuongHieuDangChon { get; set; }
    }
}
