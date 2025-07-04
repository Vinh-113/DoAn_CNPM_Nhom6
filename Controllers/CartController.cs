using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using TechStore.Models;

namespace TechStore.Controllers
{
    public class CartController : Controller
    {
        DBTechStoreEntities dBO = new DBTechStoreEntities();
        public List<CartItem> GetCart()
        {

            string usercart_id = "GioHang" + Session.SessionID;
            List<CartItem> myCart = (List<CartItem>)Session[usercart_id];
            if (myCart == null)
            {
                myCart = new List<CartItem>();
                Session[usercart_id] = myCart;
            }
            return myCart;
        }
        public ActionResult RebackDetails(int id)
        {
            return RedirectToAction("Details", "CustomerPro", new { id = id });
        }

        [HttpGet]
        public ActionResult PaymentCart()
        {
            List<CartItem> myCart = GetCart();
            ViewBag.Total = TotalMoney();
            string usermodel = (string)Session["DaDangNhap"];
            var cus = dBO.Customers.Where(s => s.NameCus == usermodel).FirstOrDefault();
            var checkout = new Payment
            {
                mycart = myCart,
                Customers = cus

            };
            return View(checkout);
        }
        [HttpPost]
        public ActionResult PaymentCart(Payment model)

        {
            if (ModelState.IsValid)
            {
                string paymentstat = ""; string paymentmethod = "";
                if (model.Order.PaymentMethod == "1")
                {
                    paymentstat = "Đã thanh toán";
                    paymentmethod = "Thanh toán qua thẻ";
                }
                else
                {
                    paymentstat = "Chưa thanh toán";
                    paymentmethod = "Thanh toán khi nhận hàng";
                }

                decimal sum = TotalMoney() + TotalMoney() * 0.10m;
                var order = new OrderPro
                {
                    IDCus = model.Customers.IDCus,
                    DateOrder = DateTime.Now,
                    TotalAmount = TotalMoney(),
                    Status = "Đã tiếp nhận thành công, đang xử lý",
                    PaymentMethod = paymentmethod,
                    TrackingNumber = GenerateTrackingNumber(),
                    AddressDeliverry = model.Customers.StreetAddress + "," + model.Customers.Ward + "," + model.Customers.District + "," + model.Customers.City,
                    DeliveryDate = DateTime.Now.AddDays(8),
                    ShippingCost = sum,
                    PaymentStatus = paymentstat
                };

                dBO.OrderProes.Add(order);
                dBO.SaveChanges();
                // Save order details
                List<CartItem> myCart = GetCart();
                foreach (var item in myCart)
                {
                    var orderDetail = new OrderDetail
                    {
                        IDOrder = order.ID,
                        IDProduct = item.ProductID,
                        Quantity = item.Number,
                        UnitPrice = (double)item.Price,
                        Subtotal = (double)item.FinalPrice(),
                        Note = "Đơn hàng đang chờ xử lý"
                    };
                    dBO.OrderDetails.Add(orderDetail);
                }
                dBO.SaveChanges();
                return RedirectToAction("PaymentSuccess", "Cart");
            }
            else
            {
                ViewBag.Error = "Đặt hàng không thành công";
                return View(model);
            }

        }
        public ActionResult PaymentSuccess()
        {
            return View();
        }
        private string GenerateTrackingNumber()
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            var result = new string(
                Enumerable.Repeat(chars, 10)
                          .Select(s => s[random.Next(s.Length)])
                          .ToArray());
            return result;
        }

        public ActionResult ShowCart()
        {
            if (Session["DaDangNhap"] == null)
            {
                return RedirectToAction("DangNhap", "User");
            }
            List<CartItem> myCart = GetCart();
            ViewBag.Total = TotalMoney();
            return View(myCart);
        }
        public ActionResult UpdateCart(int id)
        {
            Product product = dBO.Products.Find(id);
            if (product != null)
            {
                List<CartItem> cart = GetCart();
                CartItem cartItem = cart.FirstOrDefault(item => item.ProductID == id);
                cartItem.Number++;
                Session["GioHang" + Session.SessionID] = cart;
            }
            return RedirectToAction("ShowCart");
        }
        public ActionResult AddToShowCart(int id)// Dùng cho chức năng thêm giỏ hàng
        {
            //Lấy giỏ hàng hiện tại
            List<CartItem> myCart = GetCart();
            CartItem currentProduct = myCart.FirstOrDefault(p =>
            p.ProductID == id);
            if (currentProduct == null)  //Kiểm tra sản phẩm tồn tại trong giỏ hàng
            {
                currentProduct = new CartItem(id);
                myCart.Add(currentProduct);
            }
            else
            {
                currentProduct.Number++; //Sản phẩm đã có trong giỏ thì
            }
            return RedirectToAction("Details", "CustomerPro", new { id = currentProduct.ProductID });
        }
        public ActionResult AddToCart(int id) //Dùng cho chức năng đặt mua
        {
            //Lấy giỏ hàng hiện tại
            Session["currentProduct"] = id;
            List<CartItem> myCart = GetCart();
            CartItem currentProduct = myCart.FirstOrDefault(p =>
            p.ProductID == id);
            if (currentProduct == null)  //Kiểm tra sản phẩm tồn tại trong giỏ hàng
            {
                currentProduct = new CartItem(id);
                myCart.Add(currentProduct);
            }
            else
            {
                currentProduct.Number++; //Sản phẩm đã có trong giỏ thì
            }
            return RedirectToAction("ShowCart");
        }

        public ActionResult RemoveSubtractionCart(int id)
        {
            List<CartItem> myCart = GetCart();
            CartItem currentProduct = myCart.FirstOrDefault(p => p.ProductID == id);
            if (currentProduct != null)
            {
                currentProduct.Number--;
                Session["GioHang" + Session.SessionID] = myCart;
            }
            return RedirectToAction("ShowCart");
        }

        public ActionResult RemoveFromCart(int id)
        {
            List<CartItem> myCart = GetCart();
            CartItem currentProduct = myCart.FirstOrDefault(p => p.ProductID == id);
            if (currentProduct != null)
            {
                myCart.Remove(currentProduct);
                Session["GioHang" + Session.SessionID] = myCart;
            }
            return RedirectToAction("ShowCart");
        }
        //Method tính tổng tiền
        public ActionResult CartPartital()
        {
            ViewBag.TongSoLuong = TotalQuantity();
            return PartialView();
        }
        public JsonResult GetCartQuantity()
        {
            int totalQuantity = TotalQuantity();
            return Json(new { quantity = totalQuantity }, JsonRequestBehavior.AllowGet);
        }
        public decimal TotalMoney()
        {
            decimal totalPrice = 0;
            List<CartItem> myCart = GetCart();
            foreach (var item in myCart)
            {
                totalPrice += item.FinalPrice();
            }
            return totalPrice;
        }
        public int TotalQuantity()
        {
            int totalQuantity = 0;
            List<CartItem> myCart = GetCart();
            foreach (var item in myCart)
            {
                totalQuantity += item.Number;
            }
            return totalQuantity;
        }
        [HttpPost]
        public ActionResult UpdateQuantity(int ProductID)
        {
            List<CartItem> myCart = GetCart();
            CartItem currentProduct = myCart.FirstOrDefault(p => p.ProductID == ProductID);
            return RedirectToAction("ShowCart");
        }
    }
}