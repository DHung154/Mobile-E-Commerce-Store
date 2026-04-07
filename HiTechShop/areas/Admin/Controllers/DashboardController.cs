using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HiTechShop.Areas.Admin.Data;
using HiTechShop.Models;

namespace HiTechShop.Areas.Admin.Controllers
{
    [AuthorizeAdmin]
    public class DashboardController : Controller
    {
        HiTechEntities db = new HiTechEntities();

        // GET: Admin/Dashboard
        public ActionResult Index()
        {
            // Thống kê cơ bản
            var totalSanPham = db.SanPhams.Count();
            var totalDonHang = db.DonHangs.Count();
            var totalTaiKhoan = db.TaiKhoans.Count();
            var totalNguoiDung = db.NguoiDungs.Count();

            ViewBag.TotalSanPham = totalSanPham;
            ViewBag.TotalDonHang = totalDonHang;
            ViewBag.TotalTaiKhoan = totalTaiKhoan;
            ViewBag.TotalNguoiDung = totalNguoiDung;

            // Cảnh báo sản phẩm sắp hết hạn (còn 3 tháng)
            var ngayHienTai = DateTime.Now;
            var ngaySauBaThangs = ngayHienTai.AddMonths(3);

            // Lấy dữ liệu vào memory trước, rồi filter
            var sanPhamList = db.SanPhams.ToList();

            var sanPhamSapHetHan = sanPhamList
                .Where(sp => sp.HanSuDung.HasValue 
                    && sp.HanSuDung > ngayHienTai 
                    && sp.HanSuDung <= ngaySauBaThangs)
                .OrderBy(sp => sp.HanSuDung)
                .ToList();

            // Cảnh báo sản phẩm đã hết hạn
            var sanPhamDaHetHan = sanPhamList
                .Where(sp => sp.HanSuDung.HasValue && sp.HanSuDung <= ngayHienTai)
                .OrderBy(sp => sp.HanSuDung)
                .ToList();

            // Thống kê tồn kho
            var soLuongTonTatCa = sanPhamList.Sum(sp => sp.SoLuongTon) ?? 0;
            var sanPhamHetHang = sanPhamList.Where(sp => sp.SoLuongTon <= 0).Count();

            ViewBag.SanPhamSapHetHan = sanPhamSapHetHan;
            ViewBag.SanPhamDaHetHan = sanPhamDaHetHan;
            ViewBag.SoLuongTonTatCa = soLuongTonTatCa;
            ViewBag.SanPhamHetHang = sanPhamHetHang;

            // Doanh thu tháng này
            var ngayDauThang = new DateTime(ngayHienTai.Year, ngayHienTai.Month, 1);
            var doanhThuThangNay = db.DonHangs
                .Where(dh => dh.NgayDat >= ngayDauThang && dh.NgayDat <= ngayHienTai)
                .Sum(dh => dh.TongTien) ?? 0;

            ViewBag.DoanhThuThangNay = doanhThuThangNay;

            // ======================================
            // THỐNG KÊ THANH TOÁN
            // ======================================

            // Lấy tất cả đơn hàng vào memory trước
            var allDonHangs = db.DonHangs.ToList();

            // Đơn hàng đã thanh toán
            var donHangDaThanhToan = allDonHangs
                .Where(dh => dh.TrangThaiThanhToan == "Đã thanh toán" || dh.TrangThaiThanhToan == "Paid")
                .ToList();

            // Đơn hàng chưa thanh toán
            var donHangChuaThanhToan = allDonHangs
                .Where(dh => dh.TrangThaiThanhToan == "Chưa thanh toán" || dh.TrangThaiThanhToan == "Pending" || dh.TrangThaiThanhToan == null)
                .ToList();

            // Doanh thu từ các đơn hàng đã thanh toán
            var doanhThuDaThanhToan = donHangDaThanhToan.Sum(dh => dh.TongTien) ?? 0;

            // Doanh thu từ các đơn hàng chưa thanh toán
            var doanhThuChuaThanhToan = donHangChuaThanhToan.Sum(dh => dh.TongTien) ?? 0;

            ViewBag.DonHangDaThanhToan = donHangDaThanhToan.Count;
            ViewBag.DonHangChuaThanhToan = donHangChuaThanhToan.Count;
            ViewBag.DoanhThuDaThanhToan = doanhThuDaThanhToan;
            ViewBag.DoanhThuChuaThanhToan = doanhThuChuaThanhToan;

            // Top 10 đơn hàng chưa thanh toán (sắp xếp theo ngày cũ nhất)
            var donHangChuaThanhToanTop10 = donHangChuaThanhToan
                .OrderBy(dh => dh.NgayDat)
                .Take(10)
                .ToList();

            ViewBag.DonHangChuaThanhToanTop10 = donHangChuaThanhToanTop10;

            // Đơn hàng đã thanh toán tháng này
            var donHangDaThanhToanThangNay = donHangDaThanhToan
                .Where(dh => dh.NgayDat >= ngayDauThang && dh.NgayDat <= ngayHienTai)
                .Count();

            ViewBag.DonHangDaThanhToanThangNay = donHangDaThanhToanThangNay;

            return View();
        }
    }
}