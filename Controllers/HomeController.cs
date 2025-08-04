using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.DynamicData;
using System.Web.Mvc;
using TechStore.Models;

namespace TechStore.Controllers
{
    public class HomeController : Controller
    {
        DBTechStoreEntities dB = new DBTechStoreEntities();
        public ActionResult Index()
        {
            using (var db = new DBTechStoreEntities())
            {
                // Lấy tất cả sản phẩm
                var products = db.Products.ToList();

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
                foreach (var product in products)
                {
                    soldQuantities[product.ProductID] = (int)(soldItems.ContainsKey(product.ProductID) 
                        ? soldItems[product.ProductID] 
                        : 0);
                }
                
                // Truyền dữ liệu số lượng đã bán vào ViewBag
                ViewBag.SoldQuantities = soldQuantities;
                
                return View(products);
            }
        }
        [HttpPost]
        public ActionResult LogToOutput(string message)
        {
            // Log the message to the output (e.g., console, debug output, etc.)
            System.Diagnostics.Debug.WriteLine(message);
            return new EmptyResult(); // Return an empty result since this is a logging action
        }
        [HttpPost]
        public ActionResult Search(string keyword)
        {
            try
            {
                var products = dB.Products.Where(p => p.NamePro.Contains(keyword)).ToList();
                return View(products);
            }
            catch
            {
                ViewBag.Error = "Không tìm thấy sản phẩm";
                return new HttpNotFoundResult();
            }
        }

        [HttpGet]
        public ActionResult CatergoryPartial(String catergory)
        {
            try
            {
                var cate = dB.Categories.Where(s => s.NameCate == catergory).FirstOrDefault();
                if (cate != null)
                {
                    var pro = dB.Products.Where(p => p.Category == cate.IDCate).ToList();
                    if (pro.Count() < 1)  return View(new List<Product>()); 
                    return View(pro);
                }
            }
            catch
            {
                ViewBag.Error = "Không tìm thấy sản phẩm";
                return new HttpNotFoundResult();
            }
            return View();
        }
      /*  [HttpGet]
        public ActionResult CatergoryPartial_dt()
        {
            string catergory = "Smartphone";
            try
            {
                var cate = dB.Categories.Where(s => s.NameCate == catergory).FirstOrDefault();
                if (cate != null)
                {
                    var pro = dB.Products.Where(p => p.Category == cate.IDCate).ToList();
                    if (pro.Count() < 1) return View(new List<Product>());
                    return View(pro);
                }
            }
            catch
            {
                ViewBag.Error = "Không tìm thấy sản phẩm";
                return new HttpNotFoundResult();
                
            }
            return View();
        }
        [HttpGet]
        public ActionResult CatergoryPartial_PC()
        {
            string catergory = "PC";
            try
            {
                var cate = dB.Categories.Where(s => s.NameCate == catergory).FirstOrDefault();
                if (cate != null)
                {
                    var pro = dB.Products.Where(p => p.Category == cate.IDCate).ToList();
                    if (pro.Count() < 1) return View(new List<Product>());
                    return View(pro);
                }
            }
            catch
            {
                ViewBag.Error = "Không tìm thấy sản phẩm";
                return new HttpNotFoundResult();
            }
            return View();
        }
        [HttpGet]
        public ActionResult CatergoryPartial_ipad()
        {
            string catergory = "Tablet";
            try
            {
                var cate = dB.Categories.Where(s => s.NameCate == catergory).FirstOrDefault();
                if (cate != null)
                {
                    var pro = dB.Products.Where(p => p.Category == cate.IDCate).ToList();
                    if (pro.Count() < 1) return View(new List<Product>());

                    return View(pro);
                }
            }
            catch
            {
                ViewBag.Error = "Không tìm thấy sản phẩm";
                return new HttpNotFoundResult();
            }
            return View();
        }
        [HttpGet]
        public ActionResult CatergoryPartial_truewire()
        {
            string catergory = "Tai nghe";
            try
            {
                var cate = dB.Categories.Where(s => s.NameCate == catergory).FirstOrDefault();
                if (cate != null)
                {
                    var pro = dB.Products.Where(p => p.Category == cate.IDCate).ToList();
                    if (pro.Count() < 1) return View(new List<Product>());
                    return View(pro);
                }
            }
            catch
            {
                ViewBag.Error = "Không tìm thấy sản phẩm";
                return new HttpNotFoundResult();
            }
            return View();
        }
        [HttpGet]
        public ActionResult CatergoryPartial_pk()
        {
            string catergory = "Phụ kiện";
            try
            {
                var cate = dB.Categories.Where(s => s.NameCate == catergory).FirstOrDefault();
                if (cate != null)
                {
                    var pro = dB.Products.Where(p => p.Category == cate.IDCate).ToList();
                    if (pro.Count() < 1) return View(new List<Product>());

                    return View(pro);
                }
            }
            catch
            {
                ViewBag.Error = "Không tìm thấy sản phẩm";
                return View();
            }
            return View();
        }*/
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        [HttpGet]
        public JsonResult GetWishlist()
        {
            string user_name = (string)Session["DaDangNhap"];
            using (var db = new DBTechStoreEntities())
            {
                var wishlist = db.LoveProducts
                    .Where(x => x.CustomerName == user_name)
                    .Select(x => new { x.ProductID})
                    .ToList();
                return Json(wishlist, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public ActionResult AddToWishlist(int productId, string productName)
        {
            // Lấy userId từ session hoặc context (giả sử đã đăng nhập)
            string user_name = (string)Session["DaDangNhap"];
            using (var db = new DBTechStoreEntities())
            {
                // Kiểm tra đã có chưa
                var exist = db.LoveProducts.FirstOrDefault(x => x.ProductID == productId && x.CustomerName == user_name);
                if (exist == null)
                {
                    //Lấy thông tin khách hàng 
                    var cus = db.Customers.FirstOrDefault(x => x.NameCus == user_name);
                    var love = new LoveProduct
                    {
                        ProductID = productId,
                        CustomerID = cus.IDCus,
                        CustomerName = cus.NameCus
                        // nếu có cột này
                    };
                    db.LoveProducts.Add(love);
                    db.SaveChanges();
                    return Json(new { success = true, message = "Đã thêm vào yêu thích!" });
                }
                else
                {
                    return Json(new { success = false, message = "Sản phẩm đã có trong yêu thích!" });
                }
            }
        }
        // Xóa khỏi wishlist
        [HttpPost]
        public JsonResult RemoveFromWishlist(int productId)
        {
            int userId = Convert.ToInt32(Session["UserID"]);
            using (var db = new DBTechStoreEntities())
            {
                var item = db.LoveProducts.FirstOrDefault(x => x.ProductID == productId);
               try
                {
                    if (item != null)
                    {
                        db.LoveProducts.Remove(item);
                        db.SaveChanges();
                        return Json(new { success = true, message = "Đã bỏ tim!" });
                    }
                }
                catch
                {
                    return Json(new { success = false, message = "Không tìm thấy sản phẩm trong yêu thích!" });
                }
                return Json(new { success = false, message = "Không tìm thấy sản phẩm trong yêu thích!" });
            }
        }
    }
}