using System.Collections.Generic;

namespace TechStore.Models
{
    public class Payment
    {
        public OrderDetail OrderDetail { get; set; }
        public OrderPro Order { get; set; }
        public Customer Customers { get; set; }
        public List<CartItem> mycart { get; set; }

    }
}