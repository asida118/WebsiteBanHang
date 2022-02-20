using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebsiteBanHang.Models;
using System.Web.Security;
using CaptchaMvc.HtmlHelpers;
using CaptchaMvc;
using System.Data.Entity.Validation;
using System.Net;

namespace WebsiteBanHang.Controllers
{

    public class HomeController : Controller
    {
        QuanLyBanHangEntities db = new QuanLyBanHangEntities();
        
        //Trang chủ website bán hàng
        // GET: Home/Index
        [HttpGet]
        public ActionResult Index()
        {
            //Tạo các viewbag để lấy list sp từ csdl
            //List laptop mới
            var lstLTM = db.SanPhams.Where(n => n.MaLoaiSP == 1 && n.Moi == 1 && n.DaXoa == false).ToList();
            //Gán vào viewbag
            ViewBag.ListLTM = lstLTM;

            //List PC mới
            var lstPCM = db.SanPhams.Where(n => n.MaLoaiSP == 2 && n.Moi == 1 && n.DaXoa == false).ToList();
            ViewBag.ListPCM = lstPCM;

            //List dt mới
            var lstDTM = db.SanPhams.Where(n => n.MaLoaiSP == 7 && n.Moi == 1 && n.DaXoa == false).ToList();
            ViewBag.ListDTM = lstDTM;

            return View();
        }

        //Partial View Menu sản phẩm trên header
        [ChildActionOnly]
        public ActionResult MenuPartial()
        {
            //Lấy ra 1 list sanpham và truyền trực tiếp vào partial menu
            var lstSP = db.SanPhams;
            return PartialView(lstSP);
        }

        #region Đăng ký
        //Trang đăng ký thành viên
        [HttpGet]
        public ActionResult DangKy()
        {
            //đặt trùng tên viewbag giống trong bảng và gán câu hỏi vào để hiển thị lên view
            ViewBag.CauHoi = new SelectList(LoadCauHoi());
            return View();
        }

        //Dùng phương thức post để truyền data lên csdl, dùng biến tv trong model thay formcollection
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DangKy(ThanhVien tv)    
        {
            //lưu câu hỏi đã chọn trong dropdownlist vào csdl
            ViewBag.CauHoi = new SelectList(LoadCauHoi());  

            //Kiểm tra captcha hợp lệ
            if (this.IsCaptchaValid("Captcha không hợp lệ!"))   //nếu captcha hợp lệ
            {
                //kiểm tra đầu vào model hợp lệ (validation)
                if (ModelState.IsValid)
                {
                    //Gán cứng loại thành viên là khách hàng
                    tv.MaLoaiTV = 3;
                    ViewBag.ThongBao = "Thêm thành công";
                    //Thêm khách hàng vào csdl
                    //sau khi lấy được các thuộc tính trong biến tv qua các textbox thì truyền tv vào dbset ThanhViens
                    db.ThanhViens.Add(tv);
                    //Lưu thay đổi, lấy data từ dbset chuyển vào csdl
                    db.SaveChanges();
                }
                return View();
            }
            ViewBag.ThongBao = "Sai mã Captcha";
            return View();
        }
        #endregion

        //Chức năng đăng nhập
        //Dùng biến FormCollection do lấy thông tin của 1 view tĩnh (ko phải view của controller tạo ra nên ko dùng biến của Model được)
        [HttpPost]
        public ActionResult DangNhap(FormCollection f)
        {
            //Kiểm tra tên đăng nhập và mật khẩu
            string taikhoan = f["txtTenDangNhap"].ToString();   //lấy chuỗi của txtTenDangNhap
            string matKhau = f["txtMatKhau"].ToString();    //lấy chuỗi của txtMatKhau

            //Từ truy xuất tên tk và mk tạo đối tượng tv
            ThanhVien tv = db.ThanhViens.SingleOrDefault(n => n.TaiKhoan == taikhoan && n.MatKhau == matKhau);      
            if (tv != null)
            {
                //lấy ra list quyền tương ứng loaitv
                var lstQuyen = db.LoaiThanhVien_Quyen.Where(n => n.MaLoaiTV == tv.MaLoaiTV);   
                string Quyen = "";
                if (lstQuyen.Count() != 0)
                {
                    foreach (var item in lstQuyen)   //duyệt list quyền
                    {
                        Quyen += item.Quyen.MaQuyen + ",";
                    }
                    Quyen = Quyen.Substring(0, Quyen.Length - 1); //Cắt dấu ,
                    PhanQuyen(tv.TaiKhoan.ToString(), Quyen);

                    //Tạo session lưu thông tin và giá trị của đối tượng tv
                    Session["TaiKhoan"] = tv;
                    //Script dùng để reload lại trang khi đăng nhập thành công
                    return Content(@"<script>window.location.reload()</script>");   
                }
            }
            return Content("Tài khoản hoặc mật khẩu không chính xác.");
        }

        #region Sửa thông tin tài khoản

        //Hiển thị thông tin thành viên cần sửa
        [HttpGet]
        public ActionResult SuaThongTin()
        {
            if (Session["TaiKhoan"] == null)
            {
                //Trả về kết quả đường dẫn ko hợp lệ
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest); 
            }

            //Lấy thông tin thành viên từ session sau khi đăng nhập
            ThanhVien tv = (ThanhVien)Session["TaiKhoan"];
            ViewBag.CauHoi = new SelectList(LoadCauHoi());
            return View(tv);
        }

        //Xử lý sửa thông tin thành viên
        [HttpPost]
        public ActionResult SuaThongTin(ThanhVien tv)
        {
            //Kiểm tra validation các dữ liệu của tv có đúng ko
            if(ModelState.IsValid)
            {
                //tham số tv chứa thông tin đã sửa của thành viên
                //Tìm thành viên trong csdl dựa theo mãtv của đối tượng tv từ session
                ThanhVien thanhVien = db.ThanhViens.Find(tv.MaThanhVien);
                //Lưu giá trị thanhvien theo biến tv đã sửa
                db.Entry(thanhVien).CurrentValues.SetValues(tv);
                //Lưu thay đổi trong db
                db.SaveChanges();
                //Tạo session mới theo biến tv đã chỉnh sửa, để cập nhập lại thông tin của session
                Session["TaiKhoan"] = tv;

                return RedirectToAction("Index");
            }
            ViewBag.CauHoi = new SelectList(LoadCauHoi());
            return View(tv);
        }

        //Xử lý sửa mật khẩu
        [HttpPost]
        public ActionResult SuaMatKhau(FormCollection f)
        {
            string matKhau = f["MatKhau"].ToString();
            ThanhVien tv = (ThanhVien)Session["TaiKhoan"];
            tv.MatKhau = matKhau;

            if (ModelState.IsValid)
            {
                ThanhVien thanhVien = db.ThanhViens.Find(tv.MaThanhVien);
                db.Entry(thanhVien).CurrentValues.SetValues(tv);
                db.SaveChanges();
                Session["TaiKhoan"] = tv;

                return RedirectToAction("Index");
            }

            return RedirectToAction("SuaThongTin");
        }
        #endregion

        //Chức năng đăng xuất
        public ActionResult DangXuat()
        {
            //Thiết lập session là null
            Session["TaiKhoan"] = null;
            //Xóa bộ nhớ cookie
            FormsAuthentication.SignOut();
            //Nếu đang ở trang sửa thông tin thì đăng xuất ra trang chủ
            if (Request.UrlReferrer.ToString().Contains("SuaThongTin"))
            {
                return RedirectToAction("Index");
            }
            //Reload lại trang khi đăng xuất
            return Redirect(Request.UrlReferrer.ToString());
        }

        //Trang lỗi phân quyền, nếu truy cập ko đúng quyền truy cập
        public ActionResult LoiPhanquyen()
        {
            return View();
        }

        //Load câu hỏi để đưa vào dropdownlist
        public List<string> LoadCauHoi()
        {
            //tạo list câu hỏi chứa câu hỏi
            List<string> lstCauHoi = new List<string>();
            lstCauHoi.Add("Họ tên người cha bạn là gì?");
            lstCauHoi.Add("Ca sĩ mà bạn yêu thích là ai?");
            lstCauHoi.Add("Vật nuôi mà bạn yêu thích là gì?");
            lstCauHoi.Add("Sở thích của bạn là gì");
            lstCauHoi.Add("Hiện tại bạn đang làm công việc gì?");
            lstCauHoi.Add("Trường cấp ba bạn học là gì?");
            lstCauHoi.Add("Năm sinh của mẹ bạn là gì?");
            lstCauHoi.Add("Bộ phim mà bạn yêu thích là gì?");
            lstCauHoi.Add("Bài nhạc mà bạn yêu thích là gì?");

            return lstCauHoi;
        }

        public void PhanQuyen(string Taikhoan, string Quyen)
        {
            FormsAuthentication.Initialize();

            var ticket = new FormsAuthenticationTicket(1,
                                            Taikhoan,   //đặt tên ticket theo tên tk 
                                            DateTime.Now,   //lấy tgian bắt đầu
                                            DateTime.Now.AddHours(3),   //thời gian 3 tiếng out ra
                                            false,  //ko lưu
                                            Quyen,  //Lấy chuỗi phân quyền
                                            FormsAuthentication.FormsCookiePath);   //Lấy đg dẫn cookie thay vì name
            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, FormsAuthentication.Encrypt(ticket));  //tạo cookie(tự tạo name, mã hóa thông tin ticket add vào cookie)
            if (ticket.IsPersistent) cookie.Expires = ticket.Expiration;    //ktra cookie có chưa
            Response.Cookies.Add(cookie);     //
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