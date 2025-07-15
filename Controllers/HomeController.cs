using System.Linq;
using System.Web.Mvc;
using TechStore.Models;

namespace TechStore.Controllers
{
    public class HomeController : Controller
    {
        DBTechStoreEntities dB = new DBTechStoreEntities();
        public ActionResult Index()
        {
            if (Session["admin"] != null)
            {
                Session.Clear();
                Session.Abandon();
            }
            var pro = dB.Products.ToList();
            return View(pro);
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
                return View();
            }
        }

        [HttpGet]
        public ActionResult CatergoryPartial()
        {
            string catergory = "Laptop";
            try
            {
                var cate = dB.Categories.Where(s => s.NameCate == catergory).FirstOrDefault();
                if (cate != null)
                {
                    var pro = dB.Products.Where(p => p.Category == cate.IDCate).ToList();
                    return View(pro);
                }
            }
            catch
            {
                ViewBag.Error = "Không tìm thấy sản phẩm";
                return View();
            }
            return View();
        }
        [HttpGet]
        public ActionResult CatergoryPartial_dt()
        {
            string catergory = "Điện thoại";
            try
            {
                var cate = dB.Categories.Where(s => s.NameCate == catergory).FirstOrDefault();
                if (cate != null)
                {
                    var pro = dB.Products.Where(p => p.Category == cate.IDCate).ToList();
                    return View(pro);
                }
            }
            catch
            {
                ViewBag.Error = "Không tìm thấy sản phẩm";
                return View();
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
                    return View(pro);
                }
            }
            catch
            {
                ViewBag.Error = "Không tìm thấy sản phẩm";
                return View();
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
                    return View(pro);
                }
            }
            catch
            {
                ViewBag.Error = "Không tìm thấy sản phẩm";
                return View();
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
                    return View(pro);
                }
            }
            catch
            {
                ViewBag.Error = "Không tìm thấy sản phẩm";
                return View();
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
                    return View(pro);
                }
            }
            catch
            {
                ViewBag.Error = "Không tìm thấy sản phẩm";
                return View();
            }
            return View();
        }
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
    }
}