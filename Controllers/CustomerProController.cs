using TechStore.Models;
using System;
using System.Linq;
using System.Web.Mvc;

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
            if (Session["DaDangNhap"] == null)
            {
                TempData["ThongBaoLoi"] = "Bạn cần đăng nhập vào để mua";
                return RedirectToAction("DangNhap", "User");
            }
            else
            {
                var product = _context.Products.FirstOrDefault(s => s.ProductID == id);
                var relatedProducts = _context.Products.Where(s => s.Category1.IDCate == product.Category1.IDCate).ToList();//Tìm sản phẩm tương tự nhưng có mã Catrgory giống như sản phẩm hồi nãy
                var reviews = _context.Reviews.Where(s => s.ProductID == id).ToList();
                var relatedPro = new RelatedPro
                {
                    RelatedReviews = reviews,
                    Product = product,
                    RelatedProducts = relatedProducts,
                };
                return View(relatedPro);
            }
        }


        [HttpPost]
        public ActionResult CreateReview(RelatedPro pro)
        {
            var user = (string)Session["DaDangNhap"];
            var cus = _context.Customers.FirstOrDefault(s => s.NameCus == user);
            var re = new Review()
            {
                ProductID = pro.Product.ProductID,
                CustomerID = cus.IDCus ,
                Rating = pro.Review.Rating,
                ReviewContent = pro.Review.ReviewContent,
                ReviewDate = DateTime.Now,
                ReviewerName = cus.NameCus
            };
            db.Reviews.Add(re);
            db.SaveChanges();
            return RedirectToAction("Details","CustomerPro", new {id = pro.Product.ProductID});

        }
    }
}