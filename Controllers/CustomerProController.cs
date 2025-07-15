using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web.Mvc;
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
                var relatedPro = new RelatedPro
                {
                    Product = product,
                    RelatedProducts = relatedProducts,
                    RelatedReviews = reviews
                };
                return View(relatedPro);
            
        }


        [HttpPost]
        public ActionResult CreateReview(decimal score , string content, int productID)
        {
            var user = (string)Session["DaDangNhap"];
            var cus = _context.Customers.FirstOrDefault(s => s.NameCus == user);
            try { 
            var re = new Review()
            {
                ProductID =productID,
                CustomerID = cus.IDCus,
                Rating = score,
                ReviewContent = content,
                ReviewDate = DateTime.Now,
                ReviewerName = cus.NameCus
            };
                db.Reviews.Add(re);
                db.SaveChanges();
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