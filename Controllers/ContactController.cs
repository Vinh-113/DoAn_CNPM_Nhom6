using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TechStore.Models;

namespace TechStore.Controllers
{
    public class ContactController : Controller
    {
        // GET: Contact
        DBTechStoreEntities dB = new DBTechStoreEntities();
        [HttpGet]
        public ActionResult IndexSupport(int idOrder, String namecus, String email, String phone,String trackingNumber)
        {
            var date = dB.OrderProes.Where(op => op.ID == idOrder).Select(op => op.DateOrder).FirstOrDefault();
            var supportRequest = new SupportRequest
            {
                IdRequest = trackingNumber,
                CustomerName = namecus,
                Email = email,
                PhoneNumber = phone,
                OrderNumber = trackingNumber,
                PurchaseDate = date.Value.ToString("dddd,mm,yyyy")

            };
            
            ViewData["ProductNames"] = dB.OrderProes.Where(op => op.ID == idOrder).Join
                (dB.OrderDetails,op => op.ID, od => od.IDOrder, 
                (op, od) => new { od.IDProduct }).
                Select(od => dB.Products.Where(p => p.ProductID == od.IDProduct).Select(p => p.NamePro).FirstOrDefault()).ToList()
                ;
            return View(supportRequest);
        }
        [HttpPost]
        public ActionResult SendSupport(SupportRequest support)
        {
            if (ModelState.IsValid)
            {
                try {
                   /* // Double-check order status before processing
                    var order = dB.OrderProes.Where(op => op.TrackingNumber == support.OrderNumber).FirstOrDefault();

                    if (order == null)
                    {
                        ViewBag.Notify = "Failed|Không tìm thấy đơn hàng";
                        return View(support);
                    }

                    // Validate order status
                    if (order.Status != "Đã giao")
                    {
                        ViewBag.Notify = "Failed|Không thể tạo yêu cầu hỗ trợ. Đơn hàng chưa được giao thành công. Trạng thái hiện tại: " + order.Status;
                        ViewBag.OrderStatus = order.Status;
                        ViewBag.CanRequestSupport = false;
                        return View(support);
                    }*/
                    
                        var dbSupportRequest = new SupportRequest
                        {
                            IdRequest = support.IdRequest,
                            CustomerName = support.CustomerName,
                            Email = support.Email,
                            PhoneNumber = support.PhoneNumber,
                            OrderNumber = support.OrderNumber,
                            PurchaseDate = support.PurchaseDate,
                            ProductName = support.ProductName,
                            RequestType = support.RequestType,
                            Description = $"Đang xử lý|{support.Description}",
                            RequestDate = DateTime.Now
                        };

                        // Add to database
                        dB.SupportRequests.Add(dbSupportRequest);
                        dB.SaveChanges();
                        ViewBag.Notify = "Success|Đã gửi yêu cầu thành công";
                    
                }
                catch {
                    ViewBag.Notify = "Failed|Đã gửi thất bại";
                    
                }
            }
            return View(support);
        }
        private string GenerateRequestId()
        {
            //Ngẫu nhiên 5 chữ cái từ A-Z
            const string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var random = new Random();
            var result = new String(Enumerable.Repeat(letters,3).Select(s => s[random.Next(s.Length)]).ToArray());
            // REQ + RandomNumber + Chữ cái + DateTime.Now
            return "REQ" + new Random().Next(1,999).ToString() + result;
        }
    }
}