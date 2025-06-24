using System;

namespace TechStore.Models
{
    public class ReviewViewModel
    {
        public int ReviewID { get; set; }
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public int CustomerID { get; set; }
        public string CustomerName { get; set; }
        public decimal Rating { get; set; }
        public string ReviewContent { get; set; }
        public DateTime? ReviewDate { get; set; }
    }
}