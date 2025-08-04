using Newtonsoft.Json;
using System.Linq;
using System.Web.Mvc;
using TechStore.Models;
using System;
using System.Collections.Generic;

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

        // Support Request Management Actions
        public ActionResult SupportRequestManagement(string filter = "all", string search = "")
        {
            if (Session["admin"] == null)
            {
                return RedirectToAction("Login", "Admins");
            }

            var requests = dbO.SupportRequests.AsQueryable();

            // Apply search filter
            if (!string.IsNullOrEmpty(search))
            {
                requests = requests.Where(r => r.IdRequest.Contains(search) ||
                                             r.CustomerName.Contains(search) ||
                                             r.Email.Contains(search) ||
                                             r.OrderNumber.Contains(search));
            }

            // Apply status filter
            switch (filter.ToLower())
            {
                case "refund":
                    requests = requests.Where(r => r.RequestType == "Refund");
                    break;
                case "warranty":
                    requests = requests.Where(r => r.RequestType == "Warranty");
                    break;
                case "recent":
                    DateTime sevenDay_ago = DateTime.Now.AddDays(-7);
                    requests = requests.Where(r => r.RequestDate >= sevenDay_ago);
                    break;
                default:
                    // Show all
                    break;
            }

            var supportRequests = requests.OrderByDescending(r => r.RequestDate).ToList();
            
            ViewBag.Filter = filter;
            ViewBag.Search = search;
            ViewBag.TotalRequests = dbO.SupportRequests.Count();
            ViewBag.RefundRequests = dbO.SupportRequests.Count(r => r.RequestType == "Refund");
            ViewBag.WarrantyRequests = dbO.SupportRequests.Count(r => r.RequestType == "Warranty");
            //Thêm biến datetime 
            DateTime sevenDaysAgo = DateTime.Now.AddDays(-7);
            var recentRequests = dbO.SupportRequests
                .Where(r => r.RequestDate >= sevenDaysAgo)
                .ToList();
            ViewBag.RecentRequests = recentRequests.Count() ;

            return View(supportRequests);
        }

        public ActionResult SupportRequestDetails(string id)
        {
            if (Session["admin"] == null)
            {
                return RedirectToAction("Login", "Admins");
            }

            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction("SupportRequestManagement");
            }

            var request = dbO.SupportRequests.FirstOrDefault(r => r.IdRequest == id);
            if (request == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy yêu cầu hỗ trợ.";
                return RedirectToAction("SupportRequestManagement");
            }

            return View(request);
        }

        [HttpPost]
        public ActionResult ProcessSupportRequest(string requestId, string action, string reason = "")
        {
            if (Session["admin"] == null)
            {
                return Json(new { success = false, message = "Unauthorized" });
            }

            try
            {
                var request = dbO.SupportRequests.FirstOrDefault(r => r.IdRequest == requestId);
                if (request == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy yêu cầu." });
                }

                string adminName = Session["admin"].ToString();
                string statusMessage = "";

                if (action == "approve")
                {
                    statusMessage = $"Yêu cầu đã được phê duyệt bởi {adminName} vào {DateTime.Now:dd/MM/yyyy HH:mm}";
                    if (!string.IsNullOrEmpty(reason))
                    {
                        statusMessage += $". Ghi chú: {reason}";
                    }
                }
                else if (action == "reject")
                {
                    statusMessage = $"Yêu cầu đã bị từ chối bởi {adminName} vào {DateTime.Now:dd/MM/yyyy HH:mm}";
                    if (!string.IsNullOrEmpty(reason))
                    {
                        statusMessage += $". Lý do: {reason}";
                    }
                }

                // Update the request description to include status
                request.Description += $"\n\n--- TRẠNG THÁI ---\n{statusMessage}";
                
                dbO.SaveChanges();

                return Json(new { 
                    success = true, 
                    message = action == "approve" ? "Đã phê duyệt yêu cầu thành công!" : "Đã từ chối yêu cầu thành công!",
                    action = action,
                    processedAt = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                    processedBy = adminName
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
        }

        [HttpPost]
        public ActionResult DeleteSupportRequest(string requestId)
        {
            if (Session["admin"] == null)
            {
                return Json(new { success = false, message = "Unauthorized" });
            }

            try
            {
                var request = dbO.SupportRequests.FirstOrDefault(r => r.IdRequest == requestId);
                if (request == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy yêu cầu." });
                }

                dbO.SupportRequests.Remove(request);
                dbO.SaveChanges();

                return Json(new { success = true, message = "Đã xóa yêu cầu thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
        }
    }
}