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
    public class QuanLyLoaiController : Controller
    {
        QuanLyBanHangEntities db = new QuanLyBanHangEntities();

        //Trang quản lý loại sản phẩm
        // GET: QuanLyLoai
        public ActionResult Index()
        {
            return View(db.LoaiSanPhams.OrderBy(n=>n.MaLoaiSP));
        }

        //Trang tạo loại sản phẩm
        [HttpGet]
        public ActionResult TaoMoi()
        {
            return View();
        }

        //Xử lý tạo loại sản phẩm
        [HttpPost]
        public ActionResult TaoMoi(LoaiSanPham loai)
        {
            db.LoaiSanPhams.Add(loai);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        //Trang chỉnh sửa loại sản phẩm
        [HttpGet]
        public ActionResult ChinhSua(int? id)
        {
            //lấy sp cần chỉnh sửa
            if (id == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            LoaiSanPham loaisp = db.LoaiSanPhams.SingleOrDefault(n => n.MaLoaiSP == id);
            if (loaisp == null)
            {
                return HttpNotFound();
            }
          
            return View(loaisp);
        }

        //Xử lý chỉnh sửa loại sản phẩm
        [HttpPost]
        public ActionResult ChinhSua(LoaiSanPham loai)
        {
            //if(ModelState.IsValid)
            db.Entry(loai).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        //Trang xóa sản phẩm
        [HttpGet]
        public ActionResult Xoa(int? id)
        {
            //lấy sp cần chỉnh sửa
            if (id == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            LoaiSanPham loaisp = db.LoaiSanPhams.SingleOrDefault(n => n.MaLoaiSP == id);
            if (loaisp == null)
            {
                return HttpNotFound();
            }
            
            return View(loaisp);
        }

        //Xử lí xóa sản phẩm
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
                LoaiSanPham loaisp = db.LoaiSanPhams.SingleOrDefault(n => n.MaLoaiSP == id);
                if (loaisp == null)
                {
                    return HttpNotFound();
                }
                db.LoaiSanPhams.Remove(loaisp);
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                var ktTonTai = db.SanPhams.Any(p => p.MaLoaiSP == id);
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