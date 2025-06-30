using Newtonsoft.Json;
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

            var stats = dbO.OrderDetails.
          GroupBy(r => r.IDProduct)
        .Select(g => new
        {
            ProductID = g.Key,
            TotalSubtotal = g.Sum(r => r.Subtotal)
        })
        .ToList();

            ViewBag.Labels = JsonConvert.SerializeObject(stats.Select(r => r.ProductID).ToList());
            ViewBag.Data = JsonConvert.SerializeObject(stats.Select(r => r.TotalSubtotal).ToList());
            //Thống kê review
            var reviewData = dbO.Reviews.Include("Product")
        .GroupBy(r => r.ProductID)
        .Select(g => new
        {
            ProductID = g.Key,
            ReviewCount = g.Count(),
            ReviewAverage = g.Average(r => r.Rating)
        })
        .ToList();

            ViewBag.ReviewLabels = JsonConvert.SerializeObject(reviewData.Select(r => r.ProductID).ToList());
            ViewBag.ReviewData = JsonConvert.SerializeObject(reviewData.Select(r => r.ReviewCount).ToList());
            ViewBag.AverageRating = JsonConvert.SerializeObject(reviewData.Select(r => r.ReviewAverage).ToList());
            // Fetch best-selling product data

        var bestSelling = dbO.OrderDetails
        .GroupBy(od => od.IDProduct)
        .Select(g => new {
        ProductID = g.Key,
        QuantitySold = g.Sum(x => x.Quantity)
    })
    .OrderByDescending(g => g.QuantitySold)
    .Take(5) // Optional: Top 5 bestsellers
    .ToList();

            ViewBag.BestSellingLabels = bestSelling.Select(x => x.ProductID).ToList();
            ViewBag.BestSellingData = bestSelling.Select(x => x.QuantitySold).ToList();

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