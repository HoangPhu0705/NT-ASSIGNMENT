// SharedViewModels/Product/ProductDetailDto.cs
using System;
using System.Collections.Generic;
using SharedViewModels.Product;

namespace SharedViewModels.Product
{
    public class ProductDetailDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsFeatured { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; }
        
        // Collections
        public List<ProductImageDto> Images { get; set; }
        public List<ProductVariantDto> Variants { get; set; }
        public string? ImageUrl => Images?.FirstOrDefault(i => i.IsPrimary)?.ImageUrl;
        public decimal Price => Variants?.FirstOrDefault()?.Price ?? 0;
        public int Stock => Variants?.FirstOrDefault()?.Stock ?? 0;
    }
}

