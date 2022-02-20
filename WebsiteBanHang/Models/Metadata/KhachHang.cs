using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebsiteBanHang.Models
{
    //Liên kết class tv này với class tv trong Model (vì nếu update Model.edmx thì validation trong class tv của model sẽ mất hết)
    [MetadataTypeAttribute(typeof(KhachHangMetadata))]
    public partial class KhachHang
    {
        internal sealed class KhachHangMetadata
        {
            //public int MaKH { get; set; }

            //[DisplayName("Họ tên")]
            //[StringLength(50, ErrorMessage = "Tên tài khoản không quá 50 kí tự.")]
            //[Required(ErrorMessage = "Hãy nhập {0}.")]
            //public string TenKH { get; set; }

            //[DisplayName("Địa chỉ")]
            //[Required(ErrorMessage = "Hãy nhập {0}.")]
            //public string DiaChi { get; set; }

            //[DisplayName("Email")]
            //[Required(ErrorMessage = " Hãy nhập địa chỉ {0}.")]
            //[RegularExpression(@"[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,4}", ErrorMessage = "{0} không hợp lệ.")]
            //public string Email { get; set; }

            //[DisplayName("Số điện thoại")]
            //[Required(ErrorMessage = "Hãy nhập {0}.")]
            //[StringLength(10, ErrorMessage = "{0} không hợp lệ.")]
            //public string SoDienThoai { get; set; }

        }
    }
}