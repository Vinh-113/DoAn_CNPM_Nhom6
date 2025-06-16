using System.Collections.Generic;

namespace TechStore.Models
{
    public class RelatedPro
    {
        public Product Product { get; set; }
        public List<Product> RelatedProducts { get; set; }
        public List<Review> RelatedReviews { get; set; }
        public Review Review { get; set; }
        public Customer Customer { get; set; }
       
    }
}