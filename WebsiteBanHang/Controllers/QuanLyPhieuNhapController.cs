using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebsiteBanHang.Models;

namespace WebsiteBanHang.Controllers
{
    [Authorize(Roles = "QuanLy,QuanTriWeb")]
    public class QuanLyPhieuNhapController : Controller
    {
        QuanLyBanHangEntities db = new QuanLyBanHangEntities();

        //Trang chủ nhập hàng
        [HttpGet]
        public ActionResult Index()
        {
            return View(db.PhieuNhaps.OrderBy(n=>n.MaPN));
        }

        //Trang nhập đơn hàng mới
        // GET: QuanLyPhieuNhap
        [HttpGet]
        public ActionResult NhapHang()
        {
            ViewBag.MaNCC = db.NhaCungCaps;
            ViewBag.ListSanPham = db.SanPhams;
            ViewBag.NgayNhap = DateTime.Today;

            return View();
        }

        //Xử lý nhập đơn hàng mới
        [HttpPost]
        public ActionResult NhapHang(PhieuNhap model, IEnumerable<ChiTietPhieuNhap> lstModel)
        {

            ViewBag.MaNCC = db.NhaCungCaps;
            ViewBag.ListSanPham = db.SanPhams;

            model.NgayNhap = DateTime.Today;
            model.DaXoa = false;
            //Sau khi đã ktra hết dl đầu vào

            db.PhieuNhaps.Add(model);
            //save để lấy MaPN gán cho lst chitietpn
            db.SaveChanges();
            
            SanPham sp;
            foreach (var item in lstModel)
            {
                sp = db.SanPhams.Single(n => n.MaSP == item.MaSP);
                sp.SoLuongTon += item.SoLuongNhap;  //update solg tồn

                //gán MaPN cho tất cả chitietpn
                item.MaPN = model.MaPN; 
            }
            //Lưu lstmodel vào database
            db.ChiTietPhieuNhaps.AddRange(lstModel);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        //Trang chi tiết phiếu nhập
        [HttpGet]
        public ActionResult ChiTietPhieu(int? id)
        {
            if (id == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            try
            {
                PhieuNhap pn = db.PhieuNhaps.SingleOrDefault(n=>n.MaPN == id);
                if (pn == null)
                    throw new Exception($"Phiếu nhập ID={id} không tồn tại");
                var lstctpn = db.ChiTietPhieuNhaps.Where(n=>n.MaPN == id);
                ViewBag.lstctpn = lstctpn;
                return View(pn);
            }
            catch (Exception ex)
            {
                return View("BaoLoi", model: $"Lỗi truy cập dữ liệu.<br>Lý do:{ex.Message}");
            }
        }

        //Trang danh sách sản phẩm hết hàng
        [HttpGet]
        public ActionResult DSSPHetHang()
        {
            //ds sp gần hết hàng với số lượng tồn bé hơn hoặc bằng 5
            var lstSP = db.SanPhams.Where(n => n.DaXoa == false && n.SoLuongTon <= 5);
            return View(lstSP);
        }

        //Trang nhập đơn hàng cho sản phẩm gần hết
        [HttpGet]
        public ActionResult NhapHangDon(int? id)
        {
            ViewBag.MaNCC = new SelectList(db.NhaCungCaps.OrderBy(n => n.TenNCC), "MaNCC", "TenNCC");
            if(id == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            SanPham sp = db.SanPhams.SingleOrDefault(n => n.MaSP == id);
            if (sp == null)
                return HttpNotFound();
            return View(sp);
        }

        //Xử lí nhập đơn hàng cho sản phẩm gần hết
        [HttpPost]
        public ActionResult NhapHangDon(PhieuNhap model, ChiTietPhieuNhap ctpn)
        {
            ViewBag.MaNCC = new SelectList(db.NhaCungCaps.OrderBy(n => n.TenNCC), "MaNCC", "TenNCC", model.MaNCC);

            model.NgayNhap = DateTime.Now;
            model.DaXoa = false;
            db.PhieuNhaps.Add(model);
            db.SaveChanges();   //save để lấy MaPN gán cho lst chitietpn

            ctpn.MaPN = model.MaPN;
            SanPham sp = db.SanPhams.Single(n => n.MaSP == ctpn.MaSP);
            sp.SoLuongTon += ctpn.SoLuongNhap;
            db.ChiTietPhieuNhaps.Add(ctpn);
            db.SaveChanges();

            return View(sp);
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
