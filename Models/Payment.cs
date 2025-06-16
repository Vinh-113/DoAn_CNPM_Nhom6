using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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