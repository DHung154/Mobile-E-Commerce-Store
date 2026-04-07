using System;
using System.Linq;

namespace HiTechShop.Models
{
    public class GioHang
    {
        private HiTechEntities data = new HiTechEntities(); // Dùng đúng DbContext

        public int MaSP { get; set; }
        public string TenSP { get; set; }
        public string AnhBia { get; set; }
        public decimal GiaBan { get; set; }
        public int SoLuong { get; set; }
        public decimal ThanhTien => SoLuong * GiaBan;

        public GioHang(int maSP)
        {
            MaSP = maSP;

            // Tìm sản phẩm trong database
            var sp = data.SanPhams.SingleOrDefault(s => s.MaSP == maSP);
            if (sp != null)
            {
                TenSP = sp.TenSP;
                AnhBia = sp.HinhAnh;
                GiaBan = (decimal)sp.GiaBan;
                SoLuong = 1;
            }
            else
            {
                throw new Exception("Sản phẩm không tồn tại trong database.");
            }
        }
    }
}
