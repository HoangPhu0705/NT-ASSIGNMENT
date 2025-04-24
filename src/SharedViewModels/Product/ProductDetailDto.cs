// SharedViewModels/Product/ProductDetailDto.cs
using System;
using System.Collections.Generic;

namespace SharedViewModels.Product;

public class ProductDetailDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsFeatured { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; }
    // Additional properties like specifications or variants could be added here
}