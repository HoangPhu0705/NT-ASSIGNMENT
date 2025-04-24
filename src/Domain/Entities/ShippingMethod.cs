using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class ShippingMethod
    {
        [Key]   
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal Cost { get; set; }
        public int EstimatedDeliveryDays { get; set; }
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
