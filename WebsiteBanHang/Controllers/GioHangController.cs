using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using WebsiteBanHang.Models;

namespace WebsiteBanHang.Controllers
{
    public class GioHangController : Controller
    {
        QuanLyBanHangEntities db = new QuanLyBanHangEntities();

        //Hiển thị icon giỏ hàng lên phần header
        [ChildActionOnly]
        public ActionResult GioHangPartial()
        {
            if (TinhTongSoLuong() == 0) 
            {
                //ktra số lượng sp trong giỏ hàng, nếu ko có thì bằng 0
                ViewBag.TongSoLuong = 0;
                ViewBag.TongTien = 0;
                return PartialView();
            }
            //Hiển thị tổng tiền và sl sp lên trên icon giỏ hàng
            ViewBag.TongSoLuong = TinhTongSoLuong();
            ViewBag.TongTien = TinhTongTien();

            return PartialView();
        }
        
        //Trang xem giỏ hàng
        [HttpGet]
        public ActionResult XemGioHang()
        {
            //lấy giỏ hàng đã đc tạo
            List<ItemGioHang> lstGioHang = LayGioHang();
            ViewBag.TongSoLuong = TinhTongSoLuong();
            ViewBag.TongTien = TinhTongTien();

            return View(lstGioHang);
        }

        //Thêm giỏ hàng thông thường ko ajax
        [HttpGet]
        public ActionResult ThemGioHang(int MaSP,int? SoLuong ,string strURL)
        {
            //Kiểm tra sp có tồn tại trong csdl ko
            SanPham sp = db.SanPhams.SingleOrDefault(n => n.MaSP == MaSP);
            if (sp == null)
            {
                //Trang đường dẫn ko hợp lệ
                Response.StatusCode = 404;
                return null;
            }

            //Lấy giỏ hàng
            List<ItemGioHang> lstGioHang = LayGioHang();

            //Kiểm tra nếu 1 sp đã tồn tại trong giỏ hàng
            ItemGioHang spCheck = lstGioHang.SingleOrDefault(n => n.MaSP == MaSP);
            if (spCheck != null)
            {
                //nếu sp đã có trong list thì khi thêm vào giỏ hàng sẽ tăng số lượng lên
                if(SoLuong != null)
                    //Nếu sp thêm vào nhiều đơn vị
                    spCheck.SoLuong += (int)SoLuong;
                else
                    //Nếu sp thêm vào 1 đơn vị
                    spCheck.SoLuong++;
                //Kiểm tra số lượng tồn trước khi cho kh mua hàng
                if (sp.SoLuongTon < spCheck.SoLuong)
                    return View("ThongBao");
                //và đơn giá sẽ tăng theo giá sp * sl tương ứng
                spCheck.ThanhTien = spCheck.SoLuong * spCheck.DonGia;
                return Redirect(strURL);
            }

            //Nếu sp chưa tồn tại trong giỏ hàng thì thêm vào list
            //Kiểm tra sp thêm vào nhiều đơn vị
            if(SoLuong != null)
            {
                ItemGioHang itemGHSL = new ItemGioHang(MaSP,(int)SoLuong);
                if(sp.SoLuongTon < itemGHSL.SoLuong)
                    //ktra số lg tồn trc khi cho kh mua hàng
                    return View("ThongBao");
                lstGioHang.Add(itemGHSL);
                return Redirect(strURL);
            }

            //Kiểm tra sp thêm vào 1 đơn vị
            ItemGioHang itemGH = new ItemGioHang(MaSP);
            //ktra số lg tồn trc khi cho kh mua hàng
            if (sp.SoLuongTon < itemGH.SoLuong) 
                return View("ThongBao");
            lstGioHang.Add(itemGH);
            return Redirect(strURL);
        }

        //Thêm giỏ hàng thông thường có ajax
        [HttpGet]
        public ActionResult ThemGioHangAjax(int MaSP, int? SoLuong, string strURL)
        {
            //Kiểm tra sp có tồn tại trong csdl ko
            SanPham sp = db.SanPhams.SingleOrDefault(n => n.MaSP == MaSP);
            if (sp == null)
            {
                //Trang đường dẫn ko hợp lệ
                Response.StatusCode = 404;
                return null;
            }

            //Lấy giỏ hàng
            List<ItemGioHang> lstGioHang = LayGioHang();

            //Kiểm tra nếu 1 sp đã tồn tại trong giỏ hàng
            ItemGioHang spCheck = lstGioHang.SingleOrDefault(n => n.MaSP == MaSP);
            if (spCheck != null)
            {
                //nếu sp đã có trong list thì khi thêm vào giỏ hàng sẽ tăng số lượng lên
                if (SoLuong != null)
                    //Nếu sp thêm vào nhiều đơn vị
                    spCheck.SoLuong += (int)SoLuong;
                else
                    //Nếu sp thêm vào 1 đơn vị
                    spCheck.SoLuong++;
                //Kiểm tra số lượng tồn trước khi cho kh mua hàng
                if (sp.SoLuongTon < spCheck.SoLuong)
                    return Content("<script> alert(\"Sản phẩm đã hết hàng!\") </script>"); 
                //và đơn giá sẽ tăng theo giá sp * sl tương ứng
                spCheck.ThanhTien = spCheck.SoLuong * spCheck.DonGia;

                ViewBag.TongSoLuong = TinhTongSoLuong();
                ViewBag.TongTien = TinhTongTien();
                return PartialView("GioHangPartial");
            }

            //Nếu sp chưa tồn tại trong giỏ hàng thì thêm vào list
            //Kiểm tra sp thêm vào nhiều đơn vị
            if (SoLuong != null)
            {
                ItemGioHang itemGHSL = new ItemGioHang(MaSP, (int)SoLuong);
                if (sp.SoLuongTon < itemGHSL.SoLuong)
                    //ktra số lg tồn trc khi cho kh mua hàng
                    return Content("<script> alert(\"Sản phẩm đã hết hàng!\") </script>");
                lstGioHang.Add(itemGHSL);

                ViewBag.TongSoLuong = TinhTongSoLuong();
                ViewBag.TongTien = TinhTongTien();
                return PartialView("GioHangPartial");
            }

            //Kiểm tra sp thêm vào 1 đơn vị
            ItemGioHang itemGH = new ItemGioHang(MaSP);
            //ktra số lg tồn trc khi cho kh mua hàng
            if (sp.SoLuongTon < itemGH.SoLuong)
                return Content("<script> alert(\"Sản phẩm đã hết hàng!\") </script>");
            lstGioHang.Add(itemGH);

            ViewBag.TongSoLuong = TinhTongSoLuong();
            ViewBag.TongTien = TinhTongTien();
            return PartialView("GioHangPartial");
        }

        //Trang chỉnh sửa giỏ hàng ko ajax
        [HttpGet]
        public ActionResult SuaGioHang(int MaSP)
        {
            //ktra session giỏ hàng có tồn tại ko
            if(Session["GioHang"] == null)
            {
                //quay về trang chủ
                return RedirectToAction("Index", "Home");   
            }
            //ktra sp có tồn tại trong csdl ko
            SanPham sp = db.SanPhams.SingleOrDefault(n => n.MaSP == MaSP);
            if (sp == null)
            {
                //Trang đường dẫn ko hợp lệ
                Response.StatusCode = 404;
                return null;
            }

            //Lấy list giỏ hàng từ session
            List<ItemGioHang> lstGioHang = LayGioHang();

            //ktra xem sp đó có tồn tại trong giỏ hàng hay ko
            ItemGioHang spCheck = lstGioHang.SingleOrDefault(n => n.MaSP == MaSP);
            if(spCheck == null)
            {
                return RedirectToAction("Index", "Home");
            }

            //Lấy list giỏ hàng tạo giao diện
            ViewBag.GioHang = lstGioHang;

            //nếu tồn tại thì
            return View(spCheck);
        }

        //Xử lí cập nhật giỏ hàng
        [HttpPost]
        public ActionResult CapNhatGioHang(ItemGioHang itemGH)
        {
            //ktra số lượng tồn sau khi sửa
            SanPham spCheck = db.SanPhams.Single(n => n.MaSP == itemGH.MaSP);
            if(spCheck.SoLuongTon < itemGH.SoLuong)
            {
                return View("ThongBao");
            }

            //update số lg trong session giỏ hàng
            //Lấy list giỏ hàng từ sesssion giỏ hàng
            List<ItemGioHang> lstGH = LayGioHang();
            //Lấy sp cần update từ trong list giỏ hàng
            ItemGioHang itemGHUpdate = lstGH.Find(n => n.MaSP == itemGH.MaSP);  //pt find dùng để tìm các trường mong muốn
            //Update lại số lg và thành tiền
            itemGHUpdate.SoLuong = itemGH.SoLuong;
            itemGHUpdate.ThanhTien = itemGHUpdate.SoLuong * itemGHUpdate.DonGia;

            return RedirectToAction("XemGioHang");
        }

        //Xử lí xóa giỏ hàng
        [HttpGet]
        public ActionResult XoaGioHang(int MaSP)
        {
            //ktra session giỏ hàng có tồn tại ko
            if (Session["GioHang"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            //ktra sp có tồn tại trong csdl ko
            SanPham sp = db.SanPhams.SingleOrDefault(n => n.MaSP == MaSP);
            if (sp == null)
            {
                //Trang đường dẫn ko hợp lệ
                Response.StatusCode = 404;
                return null;
            }
            //Lấy list giỏ hàng từ session
            List<ItemGioHang> lstGioHang = LayGioHang();
            //ktra xem sp đó có tồn tại trong giỏ hàng hay ko
            ItemGioHang spCheck = lstGioHang.SingleOrDefault(n => n.MaSP == MaSP);
            if (spCheck == null)
            {
                return RedirectToAction("Index", "Home");
            }
            //Xóa item trong giỏ hàng
            lstGioHang.Remove(spCheck);

            return RedirectToAction("XemGioHang");
        }

        //Chức năng đặt hàng
        [HttpPost]
        public ActionResult DatHang(KhachHang kh)
        {
            //ktra session giỏ hàng có tồn tại ko
            if (Session["GioHang"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            //Kiểm tra từng loại khách hàng
            KhachHang khach = new KhachHang();
            //Nếu session tv rỗng thì là khách vãng lai
            if (Session["TaiKhoan"] == null)     
            {
                //Thêm khách hàng vào bảng khách hàng đối vs khách vãng lai
                khach = kh;
                db.KhachHangs.Add(khach);
                db.SaveChanges();
            }
            else
            {
                //Đối với khách là thành viên, thì gán dữ liệu vào khách hàng
                //Tạo biến tv lấy dữ liệu tù session và gán thông tin tương ứng
                ThanhVien tv = (ThanhVien)Session["TaiKhoan"];
                khach.TenKH = tv.HoTen;
                khach.DiaChi = tv.DiaChi;
                khach.Email = tv.Email;
                khach.SoDienThoai = tv.SoDienThoai;
                db.KhachHangs.Add(khach);
                db.SaveChanges();
            }

            //Thêm đơn hàng
            DonDatHang ddh = new DonDatHang();
            //Thêm vào makh lấy từ kh
            ddh.MaKH = int.Parse(khach.MaKH.ToString()); 
            ddh.NgayDat = DateTime.Now;
            ddh.TinhTrangGiaoHang = false;
            ddh.DaThanhToan = false;
            ddh.UuDai = 0;
            ddh.DaHuy = false;
            ddh.DaXoa = false;
            //Thêm vào bảng dondathang giá trị ddh
            db.DonDatHangs.Add(ddh);
            //Update bảng đơn đặt hàng và tạo mã ddh dùng cho chitietddh
            db.SaveChanges();   

            //Thêm chi tiết đơn đặt hàng
            List<ItemGioHang> lstGH = LayGioHang();
            //Lấy thông tin của từng sp trong giỏ hàng đưa vào chitietddh
            foreach (var item in lstGH)  
            {
                ChiTietDonDatHang ctdh = new ChiTietDonDatHang();
                //Lấy 1 mã từ ddh đã tạo trên, vì các ctddh đều cùng 1 đơn hàng
                ctdh.MaDDH = ddh.MaDDH; 
                ctdh.MaSP = item.MaSP;
                ctdh.TenSP = item.TenSP;
                ctdh.SoLuong = item.SoLuong;
                ctdh.Dongia = item.DonGia;
                db.ChiTietDonDatHangs.Add(ctdh);
            }
            db.SaveChanges();
            //Sau khi thêm vào ddh thì giỏ hàng sẽ trống
            Session["GioHang"] = null;
            return RedirectToAction("XemGioHang");
        }

        #region Methods
        //Lấy giỏ hàng
        public List<ItemGioHang> LayGioHang()
        {
            //Nếu giỏ hàng đã tồn tại, thì lấy thông tin từ session giỏ hàng
            List<ItemGioHang> lstGioHang = Session["GioHang"] as List<ItemGioHang>;
            if (lstGioHang == null)
            {
                //Nếu session ko tồn tại thì khởi tạo giỏ hàng và tạo session để lưu thông tin giỏ hàng
                lstGioHang = new List<ItemGioHang>();
                Session["GioHang"] = lstGioHang;
            }
            return lstGioHang;
        }

        // tính tổng số lg
        public double TinhTongSoLuong()
        {
            //lấy giỏ hàng
            List<ItemGioHang> lstGioHang = Session["GioHang"] as List<ItemGioHang>;
            if(lstGioHang == null)  
            {
                //nếu chưa có list giỏ hàng thì trả về gtri = 0
                return 0;
            }
            //trả về tổng số lượng của list giỏ hàng
            return lstGioHang.Sum(n => n.SoLuong); 
        }

        //tính tổng tiền
        public decimal TinhTongTien()
        {
            //lấy giỏ hàng
            List<ItemGioHang> lstGioHang = Session["GioHang"] as List<ItemGioHang>;
            if (lstGioHang == null) 
            {
                //nếu chưa có list giỏ hàng thì trả về gtri = 0
                return 0;
            }
            //trả về tổng tiền của list giỏ hàng
            return lstGioHang.Sum(n => n.ThanhTien);    
        }

        #endregion

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