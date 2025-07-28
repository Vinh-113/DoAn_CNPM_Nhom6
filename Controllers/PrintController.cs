using SelectPdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TechStore.Models;

namespace TechStore.Controllers
{
    public class PrintController : Controller
    {
        // GET: Print
        public ActionResult HTML2PDF_Order(string FileName, string viewName, int idOrder)
        {
            string html = RenderViewToString(viewName, GetHoaDonModelByOrderId(idOrder));
            HtmlToPdf converter = new HtmlToPdf();
            PdfDocument doc = converter.ConvertHtmlString(html);

            byte[] pdf = doc.Save();
            doc.Close();
            return File(pdf, "application/pdf", FileName);
        }
        // Hàm render View thành HTML string(dùng chung)
        private string RenderViewToString(string viewName, object model)
        {
            ViewData.Model = model;

            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                return sw.GetStringBuilder().ToString();
            }
        }

        public HoaDonModel GetHoaDonModelByOrderId(int orderId) //Dùng riêng cho in hóa đơn đơn hàng
        {
            using (var db = new DBTechStoreEntities())
            {
                var order = db.OrderProes.FirstOrDefault(o => o.ID == orderId);
                if (order == null) return null;

                var orderDetails = db.OrderDetails.Where(od => od.IDOrder == orderId).ToList();

                var model = new HoaDonModel
                {
                    ID = order.ID,
                    TrackingNumber = order.TrackingNumber,
                    CustomerName = order.Customer.NameCus,
                    AddressDeliverry = order.AddressDeliverry,
                    DateOrder = order.DateOrder,
                    DeliveryDate = order.DeliveryDate,
                    Status = order.Status,
                    PaymentMethod = order.PaymentMethod,
                    PaymentStatus = order.PaymentStatus,
                    TotalAmount = order.TotalAmount,
                    ShippingCost = order.ShippingCost,
                    Products = orderDetails.Select(od => new HoaDonProductModel
                    {
                        ProductName = od.Product.NamePro,
                        ImagePro = od.Product.ImagePro,
                        UnitPrice = (double)od.UnitPrice,
                        Quantity = (int)od.Quantity
                    }).ToList()
                };

                return model;
            }
        }
    }
}