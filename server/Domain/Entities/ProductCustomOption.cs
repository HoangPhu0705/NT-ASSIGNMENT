using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class ProductCustomOption
    {
        [Key]
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public Guid CustomOptionId { get; set; }
        public Product Product { get; set; }
        public CustomOption CustomOption { get; set; }
        public ICollection<CustomValue> CustomValues { get; set; } = new List<CustomValue>();
    }
}
