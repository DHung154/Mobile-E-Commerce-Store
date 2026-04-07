using HiTechShop.Areas.Admin.Data;
using HiTechShop.Areas.Admin.Data;
using HiTechShop.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace HiTechShop.Areas.Admin.Controllers
{
    [AuthorizeAdmin] // Đảm bảo class này đã được định nghĩa trong project
    public class TaiKhoanController : Controller
    {
        private HiTechEntities db = new HiTechEntities();

        // 1. Sửa hàm Index: Thêm Include để lấy thông tin bảng NguoiDung liên kết
        public ActionResult Index()
        {
            var taiKhoans = db.TaiKhoans.Include(t => t.NguoiDung).ToList();
            return View(taiKhoans);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TaiKhoanCreateVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (model.MatKhau != model.NhapLaiMatKhau)
            {
                ModelState.AddModelError("", "Mật khẩu xác nhận không khớp.");
                return View(model);
            }

            // Sử dụng Transaction để đảm bảo nếu tạo tài khoản lỗi thì người dùng cũng không được tạo
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    // Kiểm tra tên đăng nhập tồn tại
                    if (db.TaiKhoans.Any(t => t.TenDangNhap == model.TenDangNhap))
                    {
                        ModelState.AddModelError("", "Tên đăng nhập đã tồn tại.");
                        return View(model);
                    }

                    // Bước 1: Tạo mới người dùng
                    var nd = new NguoiDung
                    {
                        HoTen = model.HoTenNguoiDung,
                        NgayTao = DateTime.Now
                    };
                    db.NguoiDungs.Add(nd);
                    db.SaveChanges(); // Lưu để lấy MaND

                    // Bước 2: Tạo tài khoản
                    var tk = new TaiKhoan
                    {
                        TenDangNhap = model.TenDangNhap,
                        MatKhauHash = model.MatKhau, // Nên dùng hàm băm MD5/SHA256 tại đây
                        VaiTro = model.VaiTro,
                        MaND = nd.MaND
                    };
                    db.TaiKhoans.Add(tk);
                    db.SaveChanges();

                    dbContextTransaction.Commit();
                    TempData["SuccessMessage"] = "Tạo tài khoản thành công!";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();
                    ModelState.AddModelError("", "Có lỗi xảy ra: " + ex.Message);
                    return View(model);
                }
            }
        }

        public ActionResult Edit(int id)
        {
            var tk = db.TaiKhoans.Include(t => t.NguoiDung).FirstOrDefault(t => t.MaTK == id);
            if (tk == null)
                return HttpNotFound();

            // Lưu ý: Nếu ViewModel Edit khác TaiKhoan, hãy convert tại đây
            return View(tk);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(TaiKhoan tk)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.Entry(tk).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Cập nhật tài khoản thành công!";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Có lỗi xảy ra: " + ex.Message);
                }
            }
            return View(tk);
        }

        public ActionResult Delete(int id)
        {
            try
            {
                var tk = db.TaiKhoans.Find(id);
                if (tk != null)
                {
                    // Lưu ý: Nếu có ràng buộc khóa ngoại, hãy xóa các bảng liên quan trước
                    db.TaiKhoans.Remove(tk);
                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Xóa tài khoản thành công!";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Không thể xóa tài khoản này. Lỗi: " + ex.Message;
            }
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}