using System;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web.Mvc;
using TechStore.Models;

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
            return dbO_Cus.Customers.FirstOrDefault(s => s.NameCus == username && s.PassCus == password);
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