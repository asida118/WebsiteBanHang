using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebsiteBanHang.Models     //phải cùng namespace với class trong Model
{
    [MetadataTypeAttribute(typeof(SanPhamMetadata))]
    //liên kết class sp này với class sp trong Model (vì nếu update Model.edmx thì validation trong class sp của model sẽ mất hết)
    public partial class SanPham
    {
        internal sealed class SanPhamMetadata
        {
            public int MaSP { get; set; }

            [DisplayName("Tên sản phẩm")]
            [StringLength(100, ErrorMessage = "Tên tài khoản không quá 100 kí tự.")]
            [Required(ErrorMessage = "Hãy nhập {0}.")]
            public string TenSP { get; set; }

            [DisplayName("Đơn giá")]
            [Required(ErrorMessage = "Hãy nhập {0}.")]
            [Range(1000, 2000000000, ErrorMessage = "Giá phải từ 1000 đến 2000000000")]
            public Nullable<decimal> DonGia { get; set; }

            [DisplayName("Ngày cập nhật")]
            [Required(ErrorMessage = "Hãy nhập {0}.")]
            public Nullable<System.DateTime> NgayCapNhat { get; set; }

            [DisplayName("Cấu hình")]
            public string CauHinh { get; set; }

            [DisplayName("Hình ảnh")]
            public string HinhAnh { get; set; }

            [DisplayName("Số lượng tồn")]
            [Required(ErrorMessage = "Hãy nhập {0}.")]
            [Range(0, 1000, ErrorMessage = "Số lượng từ 0 đến 1000")]
            public Nullable<int> SoLuongTon { get; set; }

            [DisplayName("Lượt xem")]
            [Required(ErrorMessage = "Hãy nhập {0}.")]
            public Nullable<int> LuotXem { get; set; }

            [DisplayName("Lượt bình chọn")]
            [Required(ErrorMessage = "Hãy nhập {0}.")]
            public Nullable<int> LuotBinhChon { get; set; }

            [DisplayName("Lượt bình luận ")]
            [Required(ErrorMessage = "Hãy nhập {0}.")]
            public Nullable<int> LuotBinhLuan { get; set; }

            [DisplayName("Số lượt mua")]
            [Required(ErrorMessage = "Hãy nhập {0}.")]
            public Nullable<int> SoLuotMua { get; set; }

            [DisplayName("Sản phẩm mới")]
            [Required(ErrorMessage = "Hãy nhập {0}.")]
            public Nullable<int> Moi { get; set; }

            [DisplayName("Mã nhà cung cấp")]
            [Required(ErrorMessage = "Hãy chọn {0}.")]
            public Nullable<int> MaNCC { get; set; }

            [DisplayName("Mã nhà sản xuất")]
            [Required(ErrorMessage = "Hãy chọn {0}.")]
            public Nullable<int> MaNSX { get; set; }

            [DisplayName("Mã loại sản phẩm")]
            [Required(ErrorMessage = "Hãy chọn {0}.")]
            public Nullable<int> MaLoaiSP { get; set; }

            [DisplayName("Đã xóa")]
            [Required(ErrorMessage = "Hãy nhập {0}.")]
            public Nullable<bool> DaXoa { get; set; }

            [DisplayName("Hình ảnh 1")]
            public string HinhAnh1 { get; set; }

            [DisplayName("Hình ảnh 2")]
            public string HinhAnh2 { get; set; }

            [DisplayName("Hình ảnh 3")]
            public string HinhAnh3 { get; set; }

            [DisplayName("Hình ảnh 4")]
            public string HinhAnh4 { get; set; }

            [DisplayName("Mô tả")]
            public string MoTa { get; set; }
        }
    }
}