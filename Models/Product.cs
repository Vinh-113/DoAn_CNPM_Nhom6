using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations; // Add this

namespace TechStore.Models
{
    public partial class Product
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Product()
        {
            this.OrderDetails = new HashSet<OrderDetail>();
            this.Reviews = new HashSet<Review>();
        }

        public int ProductID { get; set; }

        [Required(ErrorMessage = "Tên sản phẩm là bắt buộc")]
        [Display(Name = "Tên sản phẩm ")]
        public string NamePro { get; set; }

        [Required(ErrorMessage = "Mô tả sản phẩm là bắt buộc")]
        [Display(Name = "Mô tả ")]

        public string DecriptionPro { get; set; }

        [Required(ErrorMessage = "Danh mục là bắt buộc")]
        [Display(Name = "Phân loại ")]
        public string Category { get; set; }

        [Required(ErrorMessage = "Giá là bắt buộc")]
        [Display(Name = "Giá ")]
        public Nullable<decimal> Price { get; set; }

        [Required(ErrorMessage = "Hình sản phẩm là bắt buộc")]
        public string ImagePro { get; set; }

        public Nullable<decimal> Discount { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }

        public virtual Category Category1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Review> Reviews { get; set; }
    }
}