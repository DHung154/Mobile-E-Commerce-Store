using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HiTechShop.Areas.Admin.Data;
using HiTechShop.Models;

namespace HiTechShop.Areas.Admin.Controllers
{
    [AuthorizeAdmin]
    public class LoaiSPController : Controller
    {
        HiTechEntities db = new HiTechEntities();

        // 1. SỬA HÀM INDEX: Thêm .Include để lấy được tên Danh mục
        public ActionResult Index()
        {
            var model = db.LoaiSPs.Include(l => l.DanhMuc).ToList();
            return View(model);
        }

        // GET: Admin/LoaiSP/Create
        public ActionResult Create()
        {
            ViewBag.MaDM = new SelectList(db.DanhMucs, "MaDM", "TenDM");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(LoaiSP loai)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.LoaiSPs.Add(loai);
                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Thêm loại sản phẩm thành công!";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Lỗi hệ thống: " + ex.Message);
                }
            }

            // Nếu lỗi, nạp lại ViewBag để giữ dữ liệu DropDownList
            ViewBag.MaDM = new SelectList(db.DanhMucs, "MaDM", "TenDM", loai.MaDM);
            return View(loai);
        }

        // GET: Admin/LoaiSP/Edit/5
        public ActionResult Edit(int id)
        {
            var loai = db.LoaiSPs.Find(id);
            if (loai == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy loại sản phẩm này!";
                return RedirectToAction("Index");
            }

            ViewBag.MaDM = new SelectList(db.DanhMucs, "MaDM", "TenDM", loai.MaDM);
            return View(loai);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(LoaiSP loai)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.Entry(loai).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Cập nhật loại sản phẩm thành công!";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Lỗi khi cập nhật: " + ex.Message);
                }
            }

            ViewBag.MaDM = new SelectList(db.DanhMucs, "MaDM", "TenDM", loai.MaDM);
            return View(loai);
        }

        // XỬ LÝ XÓA: Giữ nguyên logic catch lỗi khóa ngoại rất tốt của bạn
        public ActionResult Delete(int id)
        {
            try
            {
                var loai = db.LoaiSPs.Find(id);
                if (loai != null)
                {
                    db.LoaiSPs.Remove(loai);
                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Xóa loại sản phẩm thành công!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Không tìm thấy dữ liệu để xóa.";
                }
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException)
            {
                // Lỗi này xảy ra khi loại SP này đang có Sản phẩm sử dụng nó
                TempData["ErrorMessage"] = "⚠️ Không thể xóa: Hiện đang có sản phẩm thuộc loại này!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Có lỗi xảy ra: " + ex.Message;
            }
            return RedirectToAction("Index");
        }

        // Giải phóng bộ nhớ khi không sử dụng
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