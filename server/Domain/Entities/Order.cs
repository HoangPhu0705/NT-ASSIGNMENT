using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Order
    {
        [Key]
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime OrderDate { get; set; }
        public required string Status { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public required string Country { get; set; }
        public required string AddressLine1 { get; set; }
        public required string? AddressLine2 { get; set; }
        public required string City { get; set; }
        public required string ZipCode { get; set; }
        public required string State { get; set; }
        public required string MobilePhone { get; set; }
        public required Guid ShippingMethodId { get; set; }
        public required User User { get; set; }
        public ShippingMethod ShippingMethod { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}
