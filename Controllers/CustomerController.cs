﻿using System;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web.Mvc;
using TechStore.Models;

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
        [HttpPost, ActionName("Delete")]
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
        [HttpPost, ActionName("Edit")]
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
        [HttpPost, ActionName("Edit_KH")]
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
                        //Nếu đổi tên đăng nhập hay mật khẩu thì bắt buộc đăng nhập lại 
                        if (customer.NameCus != (string)Session["DaDangNhap"] || customer.PassCus != existingCustomer.PassCus)
                        {
                            return RedirectToAction("DangNhap", "User");
                        }
                        else
                        {
                            return RedirectToAction("ThongTinCaNhan", "User"); //Nếu ko thì thôi 
                        }

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
            return View();
        }

        [HttpPost]
        public ActionResult BanUser(int customerId, string reason)
        {
            try
            {
                // Find the customer and validate they exist
                var customer = dBO.Customers.FirstOrDefault(c => c.IDCus == customerId);
                if (customer == null)
                {
                    return Json(new { success = false });
                }

                // Update customer ban status
                customer.IsBanned = true;
                customer.ReasonBanned = reason;
                // Print to output console
                System.Diagnostics.Debug.WriteLine(customer.ReasonBanned);
                // Save changes
                dBO.Entry(customer).State = System.Data.Entity.EntityState.Modified;
                dBO.SaveChanges();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                // Log the exception details for debugging
                System.Diagnostics.Debug.WriteLine("Error banning user: " + ex.Message);
                return Json(new { success = false });
            }
        }

        public ActionResult UnbanUser(int id)
        {
            var customer = dBO.Customers.FirstOrDefault(c => c.IDCus == id);
            if (customer != null)
            {
                customer.IsBanned = false;
                customer.ReasonBanned = null;
                dBO.Entry(customer).State = System.Data.Entity.EntityState.Modified;
                dBO.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}