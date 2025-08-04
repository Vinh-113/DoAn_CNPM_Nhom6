using System;

namespace TechStore.Models
{
   

    public class SupportRequest
    {
        public string IdRequest { get; set; }
        public string CustomerName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string OrderNumber { get; set; }
        public  String PurchaseDate { get; set; }
        public string ProductName { get; set; }
        public string RequestType { get; set; }
        public string Description { get; set; }
        public DateTime RequestDate { get; set; } = DateTime.Now;
    }
}   