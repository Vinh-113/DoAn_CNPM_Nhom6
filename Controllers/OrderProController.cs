 using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
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
            var orderPro = db.OrderProes.Include("Customer").Where(s => s.ID == id).FirstOrDefault();
            if (orderPro == null)
            {
                return HttpNotFound();
            }
            return View(orderPro);
        }
        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var orderPro = db.OrderProes.Where(s=> s.ID == id).FirstOrDefault();
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
        public ActionResult Edit([Bind(Include = "ID,IDProduct,IDOrder,Quantity,UnitPrice,Discount,Subtotal,Note")] OrderDetail orderDetail)
        {
            if (ModelState.IsValid)
            {
                db.Entry(orderDetail).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IDOrder = new SelectList(db.OrderProes, "ID", "AddressDelivery", orderDetail.IDOrder);
            ViewBag.IDProduct = new SelectList(db.Products, "ProductID", "NamePro", orderDetail.IDProduct);
            return View(orderDetail);
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
            var item = db.OrderProes.Where(s => s.ID == id).FirstOrDefault();
            db.OrderProes.Remove(item);
            db.SaveChanges();
            return RedirectToAction("Index","OrderPro");
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
        [HttpGet]
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
        [ValidateAntiForgeryToken]
        public ActionResult ActionDelete_KH(int id)
        {
            try
            {
                var item = db.OrderProes.Where(s => s.ID == id).FirstOrDefault();
                db.OrderProes.Remove(item);
                db.SaveChanges();
            }
            catch (Exception)
            {
                TempData["Error"] = "Không thể hủy đơn được";
                
            }
            return RedirectToAction("Index_KH", "OrderPro");
        }

    }
}