using System;
using System.Collections.Generic;
using System.Linq;

namespace HiTechShop.Models
{
    public class Cart
    {
        private HiTechEntities data = new HiTechEntities(); // Dùng DbContext đúng

        public List<GioHang> list { get; set; }

        public Cart()
        {
            list = new List<GioHang>();
        }

        public Cart(List<GioHang> listGH)
        {
            list = listGH ?? new List<GioHang>();
        }

        public int SoMatHang() => list.Count;

        public int TongSLHang() => list.Sum(x => x.SoLuong);

        public decimal TongThanhTien() => list.Sum(sp => sp.ThanhTien);

        public int Them(int id)
        {
            var sp = list.FirstOrDefault(x => x.MaSP == id);
            if (sp == null)
            {
                var newItem = new GioHang(id);
                list.Add(newItem);
            }
            else
            {
                sp.SoLuong++;
            }
            return 1;
        }

        public int Giam(int id)
        {
            var sp = list.FirstOrDefault(x => x.MaSP == id);
            if (sp == null) return -1;

            sp.SoLuong--;
            if (sp.SoLuong <= 0) list.Remove(sp);

            return 1;
        }

        public int Xoa(int id)
        {
            var sp = list.FirstOrDefault(x => x.MaSP == id);
            if (sp == null) return -1;

            list.Remove(sp);
            return 1;
        }

        public void XoaHet() => list.Clear();
    }
}
