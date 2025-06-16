using TechStore.Models;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity.Infrastructure;

namespace TechStore.Controllers
{
    public class CustomerController : Controller
    {
        DBTechStoreEntities dBO = new DBTechStoreEntities();
        // GET: Customer

        public ActionResult Index()
        {
            var item = dBO.Customers.ToList();
            return View(item);
        }
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(Customer customer)
        {
            dBO.Customers.Add(customer);
            dBO.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpGet]
        public ActionResult Delete(int id)
        {
            var item = dBO.Customers.FirstOrDefault(s => s.IDCus == id);
            return View(item);
        }
        [HttpPost,ActionName("Delete")]
        public ActionResult ActionDelete(int id)
        {
            var item = dBO.Customers.Where(s => s.IDCus == id).FirstOrDefault();
            dBO.Customers.Remove(item);
            dBO.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            var item = dBO.Customers.FirstOrDefault(s => s.IDCus == id);
            return View(item);
        }
        [HttpPost,ActionName("Edit")]
        public ActionResult Edit_UP(Customer customer)
        {

            if (ModelState.IsValid)
            {
                var existingCustomer = dBO.Customers.FirstOrDefault(s => s.IDCus == customer.IDCus);
                if (existingCustomer != null)
                {
                    dBO.Entry(existingCustomer).CurrentValues.SetValues(customer);
                    try
                    {
                        dBO.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        ViewBag.Error = "Error updating customer. The customer may have been modified or deleted. Please try again.";
                    }
                }
                else
                {
                    ViewBag.Error = "Customer not found.";
                }
            }
            return View(customer);
        }


        [HttpGet]
        public ActionResult Edit_KH(int id)
        {
            var item = dBO.Customers.FirstOrDefault(s => s.IDCus == id);
            return View(item);
        }
        [HttpPost,ActionName("Edit_KH")]
        public ActionResult EditKH(Customer customer)
        {
            if (ModelState.IsValid)
            {
                var existingCustomer = dBO.Customers.FirstOrDefault(s => s.IDCus == customer.IDCus);
                if (existingCustomer != null)
                {
                    dBO.Entry(existingCustomer).CurrentValues.SetValues(customer);
                    try
                    {
                        dBO.SaveChanges();
                        return RedirectToAction("ThongTinCaNhan", "User");
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        TempData["Loi"] = "Chỉnh sửa thất bại, không biết cái lỗi gì nhưng cứ thử lại sau cái đã";
                    }
                }
                else
                {
                    TempData["Loi"] = "Không tồn tại, có vẻ như người này chưa từng đăng ký tài khoản";
                }
            }
            return RedirectToAction("ThongTinCaNhan","User");
        }
    }
}