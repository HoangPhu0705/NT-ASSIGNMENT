using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class CustomValue
    {
        [Key]
        public Guid Id { get; set; }
        public Guid ProductCustomOptionId { get; set; }
        public string Value { get; set; }
        public ProductCustomOption ProductCustomOption { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public ICollection<Cart> CartItems { get; set; } = new List<Cart>();
    }
}
