using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebsiteBanHang.Models;

namespace WebsiteBanHang.Controllers
{
    [Authorize(Roles = "QuanLy,QuanTriWeb")]
    public class QuanLyNhaSXController : Controller
    {
        QuanLyBanHangEntities db = new QuanLyBanHangEntities();
        // GET: QuanLyNhaSX

        //Trang quản lý nhà sản xuất
        public ActionResult Index()
        {
            return View(db.NhaSanXuats.OrderBy(n=>n.MaNSX));
        }

        //Trang tạo nhà sản xuất
        [HttpGet]
        public ActionResult TaoMoi()
        {
            return View();
        }

        //Xử lý tạo nhà sản xuất
        [HttpPost]
        public ActionResult TaoMoi(NhaSanXuat nsx)
        {
            db.NhaSanXuats.Add(nsx);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        //Trang chỉnh sửa nhà sản xuất
        [HttpGet]
        public ActionResult ChinhSua(int? id)
        {
            //lấy sp cần chỉnh sửa
            if (id == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            NhaSanXuat nsx = db.NhaSanXuats.SingleOrDefault(n => n.MaNSX == id);
            if (nsx == null)
            {
                return HttpNotFound();
            }

            return View(nsx);
        }

        //Xử lí chỉnh sửa nhà sản xuất
        [HttpPost]
        public ActionResult ChinhSua(NhaSanXuat nsx)
        {
            //if(ModelState.IsValid)
            db.Entry(nsx).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        //Trang xóa nhà sản xuất
        [HttpGet]
        public ActionResult Xoa(int? id)
        {
            //lấy sp cần chỉnh sửa
            if (id == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            NhaSanXuat nsx = db.NhaSanXuats.SingleOrDefault(n => n.MaNSX == id);
            if (nsx == null)
            {
                return HttpNotFound();
            }

            return View(nsx);
        }

        //Xử lí xóa nhà sản xuất
        [HttpPost]
        public ActionResult Xoa(int id)
        {
            //lấy sp cần chỉnh sửa
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            try
            {
                NhaSanXuat nsx = db.NhaSanXuats.SingleOrDefault(n => n.MaNSX == id);
                if (nsx == null)
                {
                    return HttpNotFound();
                }
                db.NhaSanXuats.Remove(nsx);
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                var ktTonTai = db.SanPhams.Any(p => p.MaNSX == id);
                if (ktTonTai)
                    return View("BaoLoi", model: $"Lỗi xóa dữ liệu.<br>Lý do: đã có sản phẩm phụ thuộc, vui lòng xóa sản phẩm trước");
                else
                    return View("BaoLoi", model: $"Lỗi xóa dữ liệu.<br>Lý do:{ex.Message}");
            }
        }

        //Giải phóng dung lượng biến db, để ở cuối controller
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (db != null)
                    db.Dispose();
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}