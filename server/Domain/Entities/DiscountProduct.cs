using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class DiscountProduct
    {
        [Key]
        public Guid Id { get; set; }
        public Guid DiscountId { get; set; }
        public Guid ProductId { get; set; }
        public Discount Discount { get; set; }
        public Product Product { get; set; }
    }
}
