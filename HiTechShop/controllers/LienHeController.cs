using HiTechShop.Models;
using System;
using System.ComponentModel;
using System.Data.Entity;
using System.Web.Mvc;
using HiTechShop.Models;

namespace HiTechShop.Controllers
{
    public class LienHeController : Controller
    {
        private HiTechEntities db = new HiTechEntities();

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db?.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GuiLienHe(LienHe model)
        {
            if (ModelState.IsValid)
            {
                model.NgayGui = DateTime.Now;

                db.LienHes.Add(model);
                db.SaveChanges();

                TempData["Success"] = "Gửi liên hệ thành công! Chúng tôi sẽ phản hồi sớm nhất.";
                return RedirectToAction("Index");
            }

            TempData["Error"] = "Gửi thất bại. Vui lòng thử lại!";
            return View("Index", model);
        }

        public ActionResult CheckLogin()
        {
            if (Session["User"] != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Login", "User");
        }

        public ActionResult HuongDanMuaHang()
        {
            return View();
        }

        public ActionResult ChinhSachDoiTra()
        {
            return View();
        }

        public ActionResult ChinhSachBaoMat()
        {
            return View();
        }
    }
}
