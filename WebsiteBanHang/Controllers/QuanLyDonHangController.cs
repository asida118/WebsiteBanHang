 using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebsiteBanHang.Models;

namespace WebsiteBanHang.Controllers
{
    //[Authorize(Roles = "QuanLy,QuanTriWeb")]
    [AllowAnonymous]
    public class QuanLyDonHangController : Controller
    {
        QuanLyBanHangEntities db = new QuanLyBanHangEntities();

        // GET: QuanLyDonHang
        public ActionResult ChuaThanhToan()
        {   
            //lấy ds đơn hàng chưa duyệt
            var lst = db.DonDatHangs.Where(n => n.DaThanhToan == false && n.DaHuy == false).OrderBy(n => n.NgayDat);
            return View(lst);
        }

        public ActionResult ChuaGiao()
        {
            //lấy ds đơn hàng chưa giao
            var lstDSDHCG = db.DonDatHangs.Where(n => n.TinhTrangGiaoHang == false && n.DaThanhToan == true && n.DaHuy == false).OrderBy(n => n.NgayGiao);
            return View(lstDSDHCG);
        }
        
        public ActionResult DaGiaoDaThanhToan()
        {
            //lấy ds đơn hàng đã giao và thanh toán
            var lstDSDHCG = db.DonDatHangs.Where(n => n.TinhTrangGiaoHang == true && n.DaThanhToan==true && n.DaHuy == false).OrderBy(n => n.NgayGiao);
            return View(lstDSDHCG);
        }
        public ActionResult DaHuy()
        {
            //lấy ds đơn hàng đã hủy
            var lstDH = db.DonDatHangs.Where(n => n.DaHuy == true).OrderBy(n => n.NgayDat);
            return View(lstDH);
        }

        [HttpGet]
        public ActionResult XemDonHang(int? id)
        {
            //ktra id hợp lệ
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            DonDatHang model = db.DonDatHangs.SingleOrDefault(n => n.MaDDH == id);
            //ktra đơn hàng có tồn tại ko
            if (model == null)
            {
                return HttpNotFound();
            }
            //hiển thị ds chitietdonhang 
            var lstChiTietDH = db.ChiTietDonDatHangs.Where(n => n.MaDDH == id);
            ViewBag.ListChiTietDH = lstChiTietDH;
            ViewBag.TenKH = model.KhachHang.TenKH;

            return View(model);
        }

        [HttpGet]
        public ActionResult DuyetDonHang(int? id)
        {
            //ktra id hợp lệ
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            DonDatHang model = db.DonDatHangs.SingleOrDefault(n => n.MaDDH == id);
            //ktra đơn hàng có tồn tại ko
            if(model ==null)
            {
                return HttpNotFound();
            }
            //hiển thị ds chitietdonhang 
            var lstChiTietDH = db.ChiTietDonDatHangs.Where(n => n.MaDDH == id);
            TempData["ListChiTietDH"] = lstChiTietDH;
            TempData.Keep();
            //ViewBag.ListChiTietDH = lstChiTietDH;
            ViewBag.TenKH = model.KhachHang.TenKH;

            return View(model);
        }

        [HttpPost]
        public ActionResult DuyetDonHang(DonDatHang ddh)
        {
            DonDatHang ddhUpdate = db.DonDatHangs.Single(n => n.MaDDH == ddh.MaDDH);    //lấy dl của đơn hàng trên
            ddhUpdate.DaThanhToan = ddh.DaThanhToan;
            ddhUpdate.TinhTrangGiaoHang = ddh.TinhTrangGiaoHang;
            ddhUpdate.DaHuy = ddh.DaHuy;
            if(ddh.TinhTrangGiaoHang == true)
            {
                ddhUpdate.NgayGiao = DateTime.Now;
            }
            
            db.SaveChanges();

            var lstChiTietDH = db.ChiTietDonDatHangs.Where(n => n.MaDDH == ddh.MaDDH);
            SanPham sp;
            if(ddh.DaThanhToan == true && ddh.TinhTrangGiaoHang == true)
            {
                foreach (var item in lstChiTietDH)
                {
                    sp = db.SanPhams.Single(n => n.MaSP == item.MaSP);
                    sp.SoLuongTon -= item.SoLuong;  //update solg tồn
                    sp.SoLuotMua++;
                }
            }
            db.SaveChanges();

            return RedirectToAction("ChuaThanhToan");
            //return View(ddhUpdate);
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