//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TechStore.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Customer
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Customer()
        {
            this.LoveProducts = new HashSet<LoveProduct>();
            this.OrderProes = new HashSet<OrderPro>();
            this.Reviews = new HashSet<Review>();
        }
    
        public int IDCus { get; set; }
        public string NameCus { get; set; }
        public string PhoneCus { get; set; }
        public string EmailCus { get; set; }
        public string PassCus { get; set; }
        public string StreetAddress { get; set; }
        public string Ward { get; set; }
        public string District { get; set; }
        public string City { get; set; }
        public Nullable<System.DateTime> DateOfBirth { get; set; }
        public Nullable<System.DateTime> RegisteredDate { get; set; }
        public Nullable<bool> IsVIP { get; set; }
        public string MembershipLevel { get; set; }
        public Nullable<bool> IsBanned { get; set; }
        public string ReasonBanned { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LoveProduct> LoveProducts { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderPro> OrderProes { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Review> Reviews { get; set; }
    }
}
