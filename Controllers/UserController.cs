using System;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web.Mvc;
using TechStore.Models;
using OtpNet;
using QRCoder;
using System.Text;
using Microsoft.Ajax.Utilities;

namespace TechStore.Controllers
{
    public class UserController : Controller
    {
        DBTechStoreEntities dbO_Cus = new DBTechStoreEntities();
        
        // GET: User
        [HttpGet]
        public ActionResult DangKy()
        {
            return View();
        }
        [HttpPost, ActionName("DangKy")]
        public ActionResult DangKy_XN(Customer customer)
        {
            if (ModelState.IsValid)
            {

                var item = dbO_Cus.Customers.Where(s => s.NameCus == customer.NameCus).FirstOrDefault();
                if (item != null)
                {
                    ViewBag.Error = "Đã có người đăng ký ";
                    return View();
                }

                customer.RegisteredDate = DateTime.Now;
                dbO_Cus.Customers.Add(customer);
                dbO_Cus.SaveChanges();
                ViewBag.Success = "Đăng ký thành công, bạn có thể đăng nhập ngay bây giờ";
                return View();
            }
            else
            {
                ViewBag.Error = "Đăng ký không thành công";
                return View();
            }
        }
       
        private static byte[] SHA1Hash(string input)
        {
            using (var sha = new System.Security.Cryptography.SHA1Managed())
            {
                var bytes = Encoding.UTF8.GetBytes(input);
                return sha.ComputeHash(bytes);
            }
        }
        [HttpPost]
        public ActionResult Factor2Setup(String cus_name,String pass)
        {
            string user = cus_name + pass;
            string issuer = "TechStoreWeb";

            // Tạo secret ngẫu nhiên
            var key = SHA1Hash(user);
            var secret = Base32Encoding.ToString(key); //chuyển key do dạng base32(SHA1) sang string

            // Tạo URL otpauth cho QR
            string otpUrl = $"otpauth://totp/{issuer}:{cus_name}?secret={secret}&issuer={issuer}&digits=6";
            // Tạo mã QR từ URL
            var qrGenerator = new QRCodeGenerator();
            var qrCodeData = qrGenerator.CreateQrCode(otpUrl, QRCodeGenerator.ECCLevel.Q); //Tạo dữ liệu qrcode
            var qrCode = new Base64QRCode(qrCodeData); //tạo qrcode
            string qrCodeImage = qrCode.GetGraphic(10);

            var model = new Face2Factor
            {
                Email = user,
                SecretKey = secret,
                QrCodeImage = qrCodeImage
            };

            return Json(new {success = true, factorModel = model});
        }
        [HttpPost]
        public ActionResult VerifyOTP(Face2Factor model, String OtpCode) 
        {
            //Khởi tạo đối tượng
            var totp = new Totp(Base32Encoding.ToBytes(model.SecretKey)); //(từ base32 sang byte , step: thời gian hiệu lực otp)
            //Nếu sai thì báo lỗi
            bool isValid = totp.VerifyTotp(OtpCode, out long timeWindowUsed, new VerificationWindow(1, 1));
            if (!isValid) return Json(new { success = false, message = "Sai mã OTP" });
            //Ngược lại
            return Json(new { success = true});
        }
       
        public ActionResult ThongTinCaNhan()
        {
            if (Session["DaDangNhap"] == null)
            {
                return RedirectToAction("DangNhap");
            }
            string name = Session["DaDangNhap"].ToString();
            ViewBag.Error = (string)TempData["Loi"];
            var customer = dbO_Cus.Customers.FirstOrDefault(s => s.NameCus == name);
            //Đếm số đơn hàng của khách hàng
            
           switch (customer)
            {
                case null:
                    ViewBag.Error = "Không tìm thấy thông tin khách hàng.";
                    return RedirectToAction("DangNhap");
                default:
                    ViewBag.SoDonHang = dbO_Cus.OrderProes.Where(s => s.IDCus == customer.IDCus).Count();
                    ViewBag.TongTien = dbO_Cus.OrderProes.Where(s => s.IDCus == customer.IDCus).Sum(s => s.TotalAmount);
                    break;
            }
            //Nếu không chưa có mua gì hết
            if ((int)ViewBag.SoDonHang == 0 || ViewBag.SoDonHang == null)
            {
                ViewBag.SoDonHang = 0;
                ViewBag.TongTien = 0;
            }
            return View(customer);
        }
        [HttpPost]
        public ActionResult SetVIP(int id, String membership, String message)
        {
            System.Diagnostics.Debug.WriteLine(message);
            bool success_bool = false;
            var customer = dbO_Cus.Customers.FirstOrDefault(c => c.IDCus == id);
            if (customer != null)
            {
                customer.IsVIP = true;
                customer.MembershipLevel = membership ;
                dbO_Cus.SaveChanges();
                success_bool = true;
                return Json(new { success = success_bool });
            }
            return Json(new { success = success_bool });
        }
        [HttpGet]
        public ActionResult DangNhap()
        {
    
            return View();
        }
        [HttpPost, ActionName("DangNhap")]
        public ActionResult DangNhapXThuc(Customer customer)
        {
            var user = ValidateUser(customer.NameCus, customer.PassCus);

            if (user != null)
            {
                if (user.IsBanned == true)
                {
                    ViewBag.ThongBao = "Không đăng nhập thành công do bạn đã bị ban, lý do là :  " + user.ReasonBanned;
                    return View();
                }
                Session["DaDangNhap"] = customer.NameCus;
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.ThongBao = "Không đăng nhập thành công ";
                return View();
            }
        }
        [HttpGet]
        public ActionResult DangXuat()
        {

            Session.Clear();
            Session.Abandon();

            return RedirectToAction("Index", "Home");
        }
        private Customer ValidateUser(string username, string password)
        {
            return dbO_Cus.Customers.FirstOrDefault(s => s.NameCus == username && s.PassCus == password );
        }
        [HttpGet]
        public ActionResult ResetPassword()
        {
            return View(); //Hiện view để đặt lại mật khẩu
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(Customer cus)
        {
            var user = dbO_Cus.Customers.FirstOrDefault(u => u.NameCus == cus.NameCus);

            if (user != null)
            {
                try
                {
                    user.PassCus = cus.PassCus;
                    dbO_Cus.Entry(user).State = System.Data.Entity.EntityState.Modified;
                    dbO_Cus.SaveChanges();
                    ViewBag.Success = "Đã reset password thành công.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    ViewBag.Error = "Reset không thành công. Có thể bị một vấn đề gì đó nên là có gì thử lại sao";
                }
            }
            else
            {
                ViewBag.Error = "Không reset được, vì không tồn tại người dùng này";
            }
            return View(); //Quay lại view cũ để nhập lại
        }

        //Reset password

    }
}