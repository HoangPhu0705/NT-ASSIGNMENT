using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Cart
    {
        [Key]
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid ProductId { get; set; }
        public Guid ProductVariantId { get; set; }
        public int Quantity { get; set; }
        public Guid? DiscountId { get; set; }
    
        public User User { get; set; }
        public Product Product { get; set; }
        public ProductVariant ProductVariant { get; set; }
        public Discount? Discount { get; set; }
    }
}
