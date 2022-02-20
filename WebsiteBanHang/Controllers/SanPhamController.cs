using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebsiteBanHang.Models;
using PagedList;
using System.Web.Mvc.Ajax;
using System.Data.Entity;

namespace WebsiteBanHang.Controllers
{
    public class SanPhamController : Controller
    {
        QuanLyBanHangEntities db = new QuanLyBanHangEntities();

        #region Partial view sản phẩm
        //Tạo 2 partial view san pham 1 va 2 để hiển thị sản phẩm theo 2 style khác nhau
        //ChildActionOnly không cho get trực tiếp vào action này
        [ChildActionOnly]
        public ActionResult SanPhamStyle1Partial() => PartialView();
        
        //ChildActionOnly không cho get trực tiếp vào action này
        [ChildActionOnly]
        public ActionResult SanPhamStyle2Partial() => PartialView();
        #endregion

        //Trang xem chi tiết chi tiết sản phẩm 
        // GET: SanPham/XemChiTiet/1
        public ActionResult XemChiTiet(int? id,string tensp)
        {
            //Check tham số truyền vào có null ko
            if(id == null)
            {
                //Trả về kết quả đường dẫn ko hợp lệ
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest); 
            }

            //Truy xuất csdl lấy ra sp với id tương ứng, trả về null nếu ko có id nào tương ứng
            SanPham sp = db.SanPhams.SingleOrDefault(n => n.MaSP == id&& n.DaXoa==false);
            
            //Tăng số lượt xem sản phẩm
            sp.LuotXem++;
            if(ModelState.IsValid)
            {
                //Update lại field lượt xem của sản phẩm
                db.SanPhams.Attach(sp);
                db.Entry(sp).Property(x => x.LuotXem).IsModified = true;
                db.SaveChanges();
            }

            if (sp == null)
            {
                //Thông báo nếu ko thấy sp này
                return HttpNotFound();
            }

            return View(sp);
        }

        //Load danh sách sp theo mã loại sp và mã nsx
        [HttpGet]
        public ActionResult SanPham(int? MaLoaiSP,int? MaNSX,int? page)
        {
            //check tham số truyền vào có null ko
            if (MaLoaiSP == null && MaNSX == null)
            {
                //Trả về kết quả đường dẫn ko hợp lệ
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //load sp theo 2 tiêu chí là mã loại sp và mã nsx
            List<SanPham> lstSP = db.SanPhams.ToList();
            if (MaNSX == null)
            {
                lstSP = db.SanPhams.Where(n => n.MaLoaiSP == MaLoaiSP).ToList();
            }
            else if(MaLoaiSP == null)
            {
                lstSP = db.SanPhams.Where(n => n.MaNSX == MaNSX).ToList();
            }else
            {
                lstSP = db.SanPhams.Where(n => n.MaLoaiSP == MaLoaiSP && n.MaNSX == MaNSX).ToList();
            }
            
            if (lstSP.Count() == 0)
            {
                //thông báo nếu ko thấy sp này
                return HttpNotFound();
            }

            //Phân trang
            if (Request.HttpMethod != "GET")
                page = 1;
            //tạo biến số sản phẩm trên trang
            int PageSize = 9;
            //tạo biến số trang hiện tại
            int PageNumber = (page ?? 1);
            ViewBag.MaLoaiSP = MaLoaiSP;
            ViewBag.MaNSX = MaNSX;
            //Lưu biến tempdata để truyền cho action timkiempartial
            TempData["DSSanPham"] = lstSP.ToList();

            return View(lstSP.OrderBy(n=>n.MaSP).ToPagedList(PageNumber,PageSize));
        }

        //Tìm kiếm sản phẩm theo từ khóa
        [HttpGet]
        public ActionResult KQTimKiem(string sTuKhoa, int? page)
        {
            //Phân trang
            if (Request.HttpMethod != "GET")
                page = 1;
            //tạo biến số sản phẩm trên trang
            int PageSize = 9;
            //tạo biến số trang hiện tại
            int PageNumber = (page ?? 1);
            //tìm kiếm theo tên sản phẩm
            var lstSP = db.SanPhams.Where(n => n.TenSP.ToUpper().Contains(sTuKhoa.ToUpper()));
            ViewBag.TuKhoa = sTuKhoa;
            //Lưu biến tempdata để truyền cho action timkiempartial
            TempData["DSSanPham"] = lstSP.ToList();

            return View(lstSP.OrderBy(n => n.TenSP).ToPagedList(PageNumber, PageSize));
        }

        //Xử lý trả về kết quả tìm kiếm
        [HttpPost]
        public ActionResult LayTuKhoaTimKiem(string sTuKhoa) => RedirectToAction("KQTimKiem", new { @sTuKhoa = sTuKhoa });

        //Tìm kiếm trong danh sách sản phẩm theo từ khóa
        public ActionResult KQTimKiemPartial(string sTuKhoaTK, int? page)
        {
            //Phân trang
            if (Request.HttpMethod != "GET")
                page = 1;
            //tạo biến số sản phẩm trên trang
            int PageSize = 9;
            //tạo biến số trang hiện tại
            int PageNumber = (page ?? 1);
            List<SanPham> lstSP = TempData["DSSanPham"] as List<SanPham>;
            //Giữ lại giá trị tempdata sau khi gửi request
            TempData.Keep("DSSanPham");
            //Tìm sản phẩm theo từ khóa trong danh sách các sp
            var lstTKSP = lstSP.Where(n => n.TenSP.ToUpper().Contains(sTuKhoaTK.ToUpper())).ToList();
            ViewBag.sTuKhoaTK = sTuKhoaTK;

            return PartialView(lstTKSP.OrderBy(n => n.TenSP).ToPagedList(PageNumber, PageSize));
        }

        public ActionResult SidebarPartial()
        {
            //Lấy ra 1 list sanpham và truyền trực tiếp vào partial menu
            var lstSP = db.SanPhams.OrderBy(n=>n.SoLuotMua);
            return PartialView(lstSP);
        }

        //Giải phóng dung lượng biến db, đặt ở cuối controller
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