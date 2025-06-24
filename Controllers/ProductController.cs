using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TechStore.Models;

namespace TechStore.Controllers
{
    public class ProductController : Controller
    {
        // GET: Product
        DBTechStoreEntities db = new DBTechStoreEntities();
        public ActionResult Index()
        {
            var item = db.Products.ToList();
            return View(item);
        }

        [HttpGet]
        public ActionResult Create()
        {
            ViewData["Category"] = new SelectList(db.Categories, "IDCate", "NameCate");
            return View();
        }

        [HttpPost]
        public ActionResult Create(Product pro, HttpPostedFileBase ImagePro)
        {
            if (ImagePro != null)
            {

                //Lưu về server
                var filename = Path.GetFileName(ImagePro.FileName);
                var path = Path.Combine(Server.MapPath("~/Images/"), filename);
                ImagePro.SaveAs(path);
                //Gán giá trị về cho bảng ImagePro
                pro.ImagePro = ImagePro.FileName;
            }
            pro.CreatedDate = System.DateTime.Now;
            pro.Price -= pro.Price * (pro.Discount / 100);
            db.Products.Add(pro);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Details(int id)
        {
            var item = db.Products.Where(s => s.ProductID == id).FirstOrDefault();
            return View(item);
        }
        [HttpGet]
        public ActionResult Edit(int id)
        {
            ViewData["Category"] = new SelectList(db.Categories, "IDCate", "NameCate");
            var item = db.Products.Where(s => s.ProductID == id).FirstOrDefault();
            return View(item);
        }
        [HttpPost, ActionName("Edit")]
        public ActionResult Edit_Up(int id, Product pro)
        {
            try
            {
                db.Entry(pro).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            catch
            {
                ViewBag.Loi = "Không thể thay đổi vì đã có sản phẩm dùng danh mục đó";
                return View();
            }
            return RedirectToAction("Index");
        }
        [HttpGet]
        public ActionResult Delete(int id)
        {
            var item = db.Products.Where(s => s.ProductID == id).FirstOrDefault();
            return View(item);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var item = db.Products.Where(s => s.ProductID == id).FirstOrDefault();
            if (item != null)
            {
                try
                {
                    db.Products.Remove(item);
                    db.SaveChanges();
                }
                catch
                {
                    ViewBag.Loi = "Không xóa được vì đã có ghi nhận trong lịch sử mua hàng";
                    return View();
                }
            }
            return RedirectToAction("Index", "Product");
        }
    }
}