using System;
using System.Collections.Generic;

namespace TechStore.Models
{
    public class HoaDonModel
    {
        public int ID { get; set; }
        public string TrackingNumber { get; set; }
        public string CustomerName { get; set; }
        public string AddressDeliverry { get; set; }
        public DateTime ? DateOrder { get; set; }
        public DateTime ? DeliveryDate { get; set; }
        public string Status { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentStatus { get; set; }
        public decimal? TotalAmount { get; set; }
        public decimal? ShippingCost { get; set; }
        public List<HoaDonProductModel> Products { get; set; }
    }

    public class HoaDonProductModel
    {
        public string ProductName { get; set; }
        public string ImagePro { get; set; }
        public double UnitPrice { get; set; }
        public int Quantity { get; set; }
        public double TotalPrice => UnitPrice * Quantity;
    }
}