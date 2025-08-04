using Microsoft.Ajax.Utilities;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using TechStore.Models;

namespace TechStore.Controllers
{
    public class OrderProController : Controller
    {
        DBTechStoreEntities db = new DBTechStoreEntities();
        // GET: OrderPro
        [HttpGet]
        public ActionResult Index()
        {
            if (Session["admin"] == null)
            {
                return RedirectToAction("Login", "Admins");
            }
            var list = db.OrderProes.Include("Customer").ToList();
            ViewBag.Error = (string)TempData["Error"];
            return View(list);
        }

        [HttpGet]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var order = db.OrderProes
        .Include(o => o.Customer)
        .Include(o => o.OrderDetails.Select(od => od.Product))
        .FirstOrDefault(o => o.ID == id);
            if (order == null)
            {
                return HttpNotFound();
            }
            OrderDetails_model orderDetails_Model = new OrderDetails_model
            {
                OrderDetail = order.OrderDetails.ToList(),
                OrderPro = order
            };
            return View(orderDetails_Model);
        }
        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var orderPro = db.OrderProes.Where(s => s.ID == id).FirstOrDefault();
            if (orderPro == null)
            {
                return HttpNotFound();
            }
            return View(orderPro);
        }

        // POST: OrderDetails/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(OrderPro orderPro)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Get the existing order from the database
                    var existingOrder = db.OrderProes.FirstOrDefault(o => o.ID == orderPro.ID);
                    if (existingOrder == null)
                    {
                        return HttpNotFound();
                    }
                    existingOrder.Status = orderPro.Status;
                    db.Entry(existingOrder).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index", "OrderPro");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Lỗi khi cập nhật: " + ex.Message);
                    TempData["Error"] = "Lỗi khi cập nhật đơn hàng: " + ex.Message;
                }
            }

            return RedirectToAction("Index", "OrderPro");
        }
        [HttpGet]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                TempData["Error"] = "Không thể hủy đơn được";
                return RedirectToAction("Index");
            }
            var item = db.OrderProes.Find(id);
            return View(item);

        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult ActionDelete(int id)
        {
            System.Diagnostics.Debug.WriteLine("Đã tìm thấy " + id);
            
            var item = db.OrderProes.Where(s => s.ID == id).FirstOrDefault();
            var or_detals = db.OrderDetails.FirstOrDefault(s => s.IDOrder == item.ID);
            if (item != null && or_detals != null)
            {
               //Nếu trong trường hợp đơn đang giao hay đã giao thì ko hủy được 
                if(item.Status != "Đang giao" || item.Status != "Đã giao")
                {
                    //Gỡ OrderDetails trước
                    db.OrderDetails.Remove(or_detals);
                    //Mói gỡ thằng này sau
                    db.OrderProes.Remove(item);
                    db.SaveChanges();
                }
                else
                {
                    return RedirectToAction("Details", "OrderPro", id);
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Không có ID" + id);
            }

            return RedirectToAction("Index", "OrderPro");
        }
        //Khách hàng xem hay hủy đơn hàng
        [HttpGet]
        public ActionResult Index_KH(int id)
        {
            if (TempData["Error"] != null)
            {
                ViewBag.Error = (string)TempData["Error"];
            }
            var list = db.OrderProes.Include("Customer").Where(s => s.IDCus == id).ToList();
            if (list == null)
                return View();
            return View(list);
        }
        /*  [HttpGet]
          public ActionResult Delete_KH(int? id)
          {
              if (id == null)
              {
                  TempData["Error"] = "Không thể hủy đơn được";
                  return RedirectToAction("Index");
              }
              var item = db.OrderProes.Find(id);
              return View(item);

          }
          [HttpPost, ActionName("Delete_KH")]
          [ValidateAntiForgeryToken]*/
        [HttpPost]
        public ActionResult Delete_KH(string id)
        {
            System.Diagnostics.Debug.WriteLine("Đã tìm thấy " + id);
            try
            {
                var item = db.OrderProes.Where(s => s.TrackingNumber == id).FirstOrDefault();
                var or_detals = db.OrderDetails.FirstOrDefault(s => s.IDOrder == item.ID);
                if (item != null && or_detals != null)
                {
                    //Nếu trong trường hợp đơn đang giao hay đã giao thì ko hủy được 
                    if (item.Status != "Đang giao" || item.Status != "Đã giao")
                    {
                        //Gỡ OrderDetails trước
                        db.OrderDetails.Remove(or_detals);
                        //Mói gỡ thằng này sau
                        db.OrderProes.Remove(item);
                        db.SaveChanges();
                    }
                    else
                    {
                        return RedirectToAction("Details_KH", "OrderPro", id);
                    }
                }
                else
                    System.Diagnostics.Debug.WriteLine("Không có ID" + id);
                
            }
            catch (Exception e)
            {
               if (e != null)
                {
                    return Json(new { success = false });
                }
            }
            return Json(new { success = true });
        }
        [HttpGet]
        public ActionResult Details_KH(int ? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var orderPro = db.OrderProes.Include("Customer").Where(s => s.ID == id).FirstOrDefault();
            var orderDetails = db.OrderDetails.Include("Product").Where(s => s.IDOrder == id).ToList();
            if (orderPro == null && orderDetails == null)
            {
                return HttpNotFound();
            }
            OrderDetails_model orderDetails_Model = new OrderDetails_model { 
                OrderDetail = orderDetails,
                OrderPro = orderPro
            };
            return View(orderDetails_Model);
        }
    }
}