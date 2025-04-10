using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace Domain.Entities
{
    public class OrderItem
    {
        [Key]
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public Guid? CustomValueId { get; set; }
        public Guid? DiscountId { get; set; }
        public decimal DiscountedPrice { get; set; }
        public Order Order { get; set; }
        public Product Product { get; set; }
        public CustomValue? CustomValue { get; set; }
        public Discount? Discount { get; set; }
    }
}
