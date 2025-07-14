using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using TechStore.Models;

namespace TechStore.Controllers
{
    public class ReviewsController : Controller
    {
        private DBTechStoreEntities db = new DBTechStoreEntities();

        // GET: Reviews
        public ActionResult Index()
        {
            var reviews = from review in db.Reviews
                          join customer in db.Customers on review.CustomerID equals customer.IDCus
                          join product in db.Products on review.ProductID equals product.ProductID
                          select new ReviewViewModel
                          {
                              ReviewID = review.ReviewID,
                              ProductID = product.ProductID,
                              ProductName = product.NamePro,
                              CustomerID = customer.IDCus,
                              CustomerName = customer.NameCus,
                              Rating = review.Rating,
                              ReviewContent = review.ReviewContent,
                              ReviewDate = review.ReviewDate,
                              IsHidden = review.IsHidden ?? false // Default to false if IsHidden is null
                          };

            return View(reviews.ToList());
        }

        // GET: Reviews/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Review review = db.Reviews.Find(id);
            if (review == null)
            {
                return HttpNotFound();
            }
            return View(review);
        }

        //// GET: Reviews/Create
        //public ActionResult Create()
        //{
        //    ViewBag.CustomerID = new SelectList(db.Customers, "IDCus", "NameCus");
        //    ViewBag.ProductID = new SelectList(db.Products, "ProductID", "NamePro");
        //    return View();
        //}

        //// POST: Reviews/Create
        //// To protect from overposting attacks, enable the specific properties you want to bind to, for 
        //// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "ReviewID,ProductID,CustomerID,Rating,ReviewContent,ReviewDate")] Review review)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Reviews.Add(review);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    ViewBag.CustomerID = new SelectList(db.Customers, "IDCus", "NameCus", review.CustomerID);
        //    ViewBag.ProductID = new SelectList(db.Products, "ProductID", "NamePro", review.ProductID);
        //    return View(review);
        //}

        // GET: Reviews/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Review review = db.Reviews.Find(id);
            if (review == null)
            {
                return HttpNotFound();
            }
            ViewBag.CustomerID = new SelectList(db.Customers, "IDCus", "NameCus", review.CustomerID);
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "NamePro", review.ProductID);
            return View(review);
        }

        // POST: Reviews/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ReviewID,ProductID,CustomerID,Rating,ReviewContent,ReviewDate")] Review review)
        {
            if (ModelState.IsValid)
            {
                db.Entry(review).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CustomerID = new SelectList(db.Customers, "IDCus", "NameCus", review.CustomerID);
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "NamePro", review.ProductID);
            return View(review);
        }

        // GET: Reviews/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Review review = db.Reviews.Find(id);
            if (review == null)
            {
                return HttpNotFound();
            }
            return View(review);
        }

        // POST: Reviews/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult HiddenConfirmed(int id)
        {
            if (ModelState.IsValid)
            {
                var review = db.Reviews.Find(id);
                if (review != null)
                {
                    // xem ẩn thay vì xóa
                    review.IsHidden = true; 
                }
                else
                {
                    return HttpNotFound();
                }
                db.Entry(review).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            /*Review review = db.Reviews.Find(id);
            db.Reviews.Remove(review);
            db.SaveChanges();*/
            return RedirectToAction("Index");
        }
        [HttpPost]
        public ActionResult ShowReview(int id)
        {
            if (ModelState.IsValid)
            {
                var review = db.Reviews.Find(id);
                if (review != null)
                {
                    // xem ẩn thay vì xóa
                    review.IsHidden = false;
                }
                else
                {
                    return Json(new { success = false });
                }
                db.Entry(review).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true });  
            }
            /*Review review = db.Reviews.Find(id);
            db.Reviews.Remove(review);
            db.SaveChanges();*/
            return RedirectToAction("Index");
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
