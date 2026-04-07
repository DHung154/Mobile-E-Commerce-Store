using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HiTechShop.Models;

namespace HiTechShop.Controllers
{
    public class HomeController : Controller
    {
       HiTechEntities data = new HiTechEntities();
        public ActionResult Index(int? maTH)
        {
              var spHot = data.SanPhams
                 .OrderByDescending(x => x.GiaBan)
                    .Take(10)
                    .ToList();
        
              var spSale = data.SanPhams
                    .Where(x => x.GiamGia > 0)
                    .Take(10)
                    .ToList();

             var spNew = data.SanPhams
                 .OrderByDescending(x => x.MaSP)
                 .Take(10)
                 .ToList();
            var thuongHieuList = data.ThuongHieux.ToList();
            List<SanPham> spTheoTH;

            ThuongHieu thChon = null;

            if (maTH == null)
            {
                spTheoTH = new List<SanPham>(); // chưa chọn thì để trống
            }
            else
            {
                spTheoTH = data.SanPhams.Where(s => s.MaTH == maTH).ToList();
                thChon = data.ThuongHieux.Find(maTH);
            }
            var viewModel = new HomeViewModel
            {
                DsSanPham = data.SanPhams
             .OrderByDescending(s => s.MaSP)
             .Take(20)
             .ToList(),

                HotProducts = spHot,
                SaleProducts = spSale,
                NewProducts = spNew,

                ThuongHieuList = thuongHieuList,
                SanPhamTheoTH = spTheoTH,
                ThuongHieuDangChon = thChon
            };

            return View(viewModel);
        }


        public ActionResult DanhMucSP()
        {
            var danhMucs = data.DanhMucs.ToList();
            return View(danhMucs);
        }


        public ActionResult LoaiSP()
        {
            var loai = data.LoaiSPs.ToList();
            return View(loai);
        }
        public ActionResult SanPhamTheoDanhMuc(int maDM)
        {
            // Lấy tất cả danh mục, loại, thương hiệu (để hiển thị menu/layout)
            var danhMuc = data.DanhMucs.ToList();
            var loaiSP = data.LoaiSPs.ToList();
            var thuongHieu = data.ThuongHieux.ToList();

            // 1. Lọc sản phẩm theo MaDM
            var sanPham = data.SanPhams
                .Where(sp => sp.MaDM == maDM)
                // Đảm bảo ToList() được gọi để thực thi truy vấn
                .ToList();

            // 2. Tạo Model, đảm bảo DsSanPham không bao giờ là null
            var model = new HomeViewModel
            {
                // Sử dụng toán tử ?? để đảm bảo nó là List<SanPham> rỗng nếu có lỗi truy vấn
                DsSanPham = sanPham ?? new List<SanPham>(),
                DSDanhMuc = danhMuc,
                DsLoaiSP = loaiSP,

            };

            // 3. Truyền tên danh mục qua ViewBag (lấy từ danh sách vừa lấy)
            ViewBag.TenDanhMuc = danhMuc.FirstOrDefault(dm => dm.MaDM == maDM)?.TenDM ?? "Không xác định";

            // Trả về View Index (vì View Index của bạn đã được sửa để nhận HomeViewModel)
            return View(model);
        }


        public ActionResult SanPhamTheoLoai(int id)
        {
            int maLoai = id;

            var sp = data.SanPhams.Where(s => s.MaLoai == maLoai).ToList();
            var loai = data.LoaiSPs.FirstOrDefault(l => l.MaLoai == maLoai);

            ViewBag.TenLoai = loai?.TenLoai;

            return View(sp);

        }




        // Action để render menu chính
        [ChildActionOnly]
        public ActionResult MenuChinh()
        {
            // Lấy danh sách danh mục từ database (mã 1-6)
            var danhMucList = data.DanhMucs
                .Where(dm => dm.MaDM >= 1 && dm.MaDM <= 6)
                .OrderBy(dm => dm.MaDM)
                .ToList();

            return PartialView("_MenuChinh", danhMucList);
        }

        // Action để lấy loại sản phẩm theo danh mục (đã có sẵn)
        [ChildActionOnly]
        public ActionResult MenuLoai(int maDM)
        {
            var loaiList = data.LoaiSPs
                .Where(l => l.MaDM == maDM)
                .ToList();

            return PartialView("_MenuLoai", loaiList);
        }

        // Action để lấy tất cả danh mục (cho dropdown DANH MỤC SẢN PHẨM)
        [ChildActionOnly]
        public ActionResult MenuDanhMuc()
        {
            var danhMucList = data.DanhMucs.ToList();
            return PartialView("_MenuDanhMuc", danhMucList);
        }




        public ActionResult ChiTietSP(int maSP)
        {
            // 1. Lấy thông tin sản phẩm dựa trên MaSP
            var sanPham = data.SanPhams
                              .FirstOrDefault(sp => sp.MaSP == maSP);

            // Xử lý trường hợp không tìm thấy sản phẩm
            if (sanPham == null)
            {
                return HttpNotFound(); // Trả về lỗi 404
            }

            // 2. Lấy danh sách đánh giá cho sản phẩm này
            var danhGiaList = data.DanhGias
                                  .Where(dg => dg.MaSP == maSP)
                                  .OrderByDescending(dg => dg.NgayDanhGia)
                                  .ToList();

            // 3. (Tùy chọn) Lấy sản phẩm liên quan (cùng loại hoặc cùng danh mục)
            var spLienQuan = data.SanPhams
                                 .Where(s => s.MaLoai == sanPham.MaLoai && s.MaSP != maSP)
                                 .Take(4)
                                 .ToList();

            ViewBag.DanhSachDanhGia = danhGiaList;
            ViewBag.SanPhamLienQuan = spLienQuan;
            ViewBag.TenDanhMuc = sanPham.DanhMuc?.TenDM ?? "Không xác định";
            ViewBag.TenThuongHieu = sanPham.ThuongHieu?.TenTH ?? "Không xác định";


            return View(sanPham); // Truyền đối tượng SanPham làm Model chính
        }

        public ActionResult TatCaSanPham()
        {
            var list = data.SanPhams.OrderByDescending(x => x.MaSP).ToList();
            return View(list);
        }
        public ActionResult SanPhamHot()
        {
            var list = data.SanPhams
                .OrderByDescending(x => x.GiaBan)
                .ToList();

            return View(list);
        }
        public ActionResult SanPhamSale()
        {
            var list = data.SanPhams
                .Where(x => x.GiamGia > 0)
                .OrderByDescending(x => x.GiamGia)
                .ToList();

            return View(list);
        }



        public ActionResult TimKiem(string keyword)
        {

            keyword = keyword?.Trim();


            if (string.IsNullOrEmpty(keyword))
                return View(new List<SanPham>());
            var ketQua = data.SanPhams
                 .Where(sp =>
                     sp.TenSP.Contains(keyword) ||
                     sp.ThuongHieu.TenTH.Contains(keyword) ||
                     sp.LoaiSP.TenLoai.Contains(keyword)
                 )
                 .ToList();

            ViewBag.TuKhoa = keyword;

            return View(ketQua);

        }


        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}