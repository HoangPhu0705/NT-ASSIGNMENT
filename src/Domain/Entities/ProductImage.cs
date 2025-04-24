using Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class ProductImage
    {   
        [Key]   
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public Guid? ProductVariantId { get; set; }
        public string ImageUrl { get; set; }
        public bool IsPrimary { get; set; }
        public string? Description { get; set; }
    
        public Product Product { get; set; }
        public ProductVariant ProductVariant { get; set; }
    }
}
