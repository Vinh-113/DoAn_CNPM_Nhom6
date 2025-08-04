using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web.Mvc;
using System.Web.Services.Description;
using TechStore.Models;

namespace TechStore.Controllers
{
    public class CustomerProController : Controller
    {
        // GET: CustomerPro
        DBTechStoreEntities db = new DBTechStoreEntities();
        private readonly ApplicationDbContext _context = new ApplicationDbContext();
        [HttpGet]
        public ActionResult Details(int id)
        {
                
                var product = _context.Products.FirstOrDefault(s => s.ProductID == id);
                var relatedProducts = _context.Products.Where(s => s.Category1.IDCate == product.Category1.IDCate).ToList();//Tìm sản phẩm tương tự nhưng có mã Catrgory giống như sản phẩm hồi nãy
                var reviews = _context.Reviews.Where(s => s.ProductID == id && s.IsHidden == false).ToList();

            // Tính toán số lượng đã bán cho mỗi sản phẩm
            var soldQuantities = new Dictionary<int, int>();

            // Query để lấy số lượng đã bán từ OrderDetails
            var soldItems = db.OrderDetails
                .Join(db.OrderProes,
                    od => od.IDOrder,
                    op => op.ID,
                    (od, op) => new { od.IDProduct, od.Quantity })
                .GroupBy(x => x.IDProduct)
                .Select(g => new {
                    ProductID = g.Key,
                    TotalSold = g.Sum(x => x.Quantity)
                })
                .ToDictionary(x => x.ProductID, x => x.TotalSold);

            // Khởi tạo mặc định số lượng bán = 0 cho tất cả sản phẩm
            foreach (var pro in db.Products)
            {
                soldQuantities[pro.ProductID] = (int)(soldItems.ContainsKey(pro.ProductID)
                    ? soldItems[pro.ProductID]
                    : 0);
            }
            var relatedPro = new RelatedPro
                {
                    Product = product,
                    RelatedProducts = relatedProducts,
                    RelatedReviews = reviews,
                    SoldItem = soldQuantities
                };
                return View(relatedPro);
            
        }


        [HttpPost]
        public ActionResult CreateReview(decimal score , string content, int productID)
        {
            //Nếu chưa đăng nhập thì buộc không báo lỗi 
            if (ViewBag["DaDangNhap"] == null)
            {
                return Json(new { success = false, message = "Bạn phải đăng nhập mới ĐG sản phẩm này mới được" });
            }
            var user = (string)Session["DaDangNhap"];
            var cus = _context.Customers.FirstOrDefault(s => s.NameCus == user);
            // Kiểm tra khách hàng đã mua sản phẩm này chưa
            var customerHasPurchased = db.OrderProes
                .Where(op => op.Status == "Đã giao" && op.IDCus == cus.IDCus)
                .Join(db.OrderDetails,
                    op => op.ID,
                    od => od.IDOrder,
                    (op, od) => od.IDProduct)
                .Any(productId => productId == productID);
            try { 
           //Kiểm tra số lượng sản phẩm đã mua nếu mua rôi thì cho review 
                 if (customerHasPurchased)
                {
                    var re = new Review()
                    {
                        ProductID = productID,
                        CustomerID = cus.IDCus,
                        Rating = score,
                        ReviewContent = content,
                        ReviewDate = DateTime.Now,
                        ReviewerName = cus.NameCus,
                        IsHidden = false //Mặc định 
                    };
                    db.Reviews.Add(re);
                    db.SaveChanges();
                }
                else return Json(new { success = false, message = "Bạn phải hoàn thành đơn có sản phẩm này mới được" });
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Bị lỗi khi up review " + ex);
                return Json(new { success = false});
            }
            return Json(new { success = true });

        }
    }
}