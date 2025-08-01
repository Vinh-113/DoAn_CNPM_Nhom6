﻿using Newtonsoft.Json;
using System.Linq;
using System.Web.Mvc;
using TechStore.Models;

namespace TechStore.Controllers
{
    public class AdminsController : Controller
    {
        // GET: Admins

        DBTechStoreEntities dbO = new DBTechStoreEntities();
        public ActionResult Index()
        {
            if (Session["admin"] == null)
            {
                return RedirectToAction("Login", "Admins");
            }
            string admin = (string)Session["admin"];
            ViewBag.Admin = admin;
            return View();
        }
        public ActionResult Statistics()
        {

            var stats = dbO.OrderDetails
    .GroupBy(r => r.IDProduct)
    .Select(g => new
    {
        Product = g.Key,
        Subtotal = g.Sum(x => x.Subtotal)
    }).Join(dbO.Products, bs => bs.Product, p => p.ProductID, (bs, p) => new
    {
        ProductName = p.NamePro,
        Subtotal = bs.Subtotal

    })
    .ToList();

            ViewBag.StatsLabels = JsonConvert.SerializeObject(stats.Select(r => r.ProductName).ToList());
            ViewBag.StatsData = JsonConvert.SerializeObject(stats.Select(r => r.Subtotal).ToList());
            //Thống kê review, bs là bên trái của bảng select , p là của bảng ngoài 
            var reviewData = dbO.Reviews
        .GroupBy(r => r.ProductID)
        .Select(g => new
        {
            ProductID = g.Key,
            ReviewCount = g.Count(),
            ReviewAverage = g.Average(r => r.Rating)
        }).Join(dbO.Products, bs => bs.ProductID , p => p.ProductID, (bs,p) => new
        {
            ProductName = p.NamePro,
            ReviewCount = bs.ReviewCount,
            ReviewAverage = bs.ReviewAverage   

        })
        .ToList();

            ViewBag.ReviewLabels = JsonConvert.SerializeObject(reviewData.Select(r => r.ProductName).ToList());
            ViewBag.ReviewData = JsonConvert.SerializeObject(reviewData.Select(r => r.ReviewCount).ToList());
            ViewBag.AverageRating = JsonConvert.SerializeObject(reviewData.Select(r => r.ReviewAverage).ToList());
            // Fetch best-selling product data

            var bestSelling = dbO.OrderDetails
    .GroupBy(od => od.IDProduct)
    .Select(g => new
    {
        ProductID = g.Key,
        QuantitySold = g.Sum(x => x.Quantity)
    })
    .Join(dbO.Products, 
          bs => bs.ProductID, 
          p => p.ProductID, 
          (bs, p) => new
          {
              ProductName = p.NamePro,
              QuantitySold = bs.QuantitySold
          })
    .Take(5)
    .ToList();

            ViewBag.BestSellingLabels = bestSelling.Select(x => x.ProductName).ToList();
            ViewBag.BestSellingData = bestSelling.Select(x => x.QuantitySold).ToList();
            //Tổng doanh thu 
            ViewBag.TotalRevenue = dbO.OrderDetails.Sum(od => od.Subtotal);
            ViewBag.TotalOrders = dbO.OrderProes.Count();
            ViewBag.AvgRating = dbO.Reviews.Any() ? dbO.Reviews.Average(r => r.Rating) : 0;
            ViewBag.TotalCustomers = dbO.Customers.Count();


            return View();
        }


        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost, ActionName("Login")]
        public ActionResult LoginConfirm(AdminUser admin)
        {
            var user = ValidateUser(admin.NameUser, admin.PasswordUser);
            // Add your authentication logic here
            if (user != null)
            {

                // Redirect to a different page on successful login
                ViewBag.ThongBao = "Chúc mừng đăng nhập thành công ";
                Session["admin"] = admin.NameUser;
                return RedirectToAction("Index", "Admins");
            }
            else
            {
                ViewBag.ThongBao = "Không đăng nhập thành công ";
                return View();
            }
        }
        private AdminUser ValidateUser(string username, string password)
        {
            return dbO.AdminUsers.FirstOrDefault(s => s.NameUser == username && s.PasswordUser == password);
        }
        public ActionResult ProductManament()
        {

            return View();
        }

        public ActionResult CateManament()
        {
            return View();
        }

        public ActionResult OderManagement()
        {
            return View();
        }
    }
}