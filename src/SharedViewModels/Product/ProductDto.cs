// SharedViewModels/Product/ProductDto.cs
using System;

namespace SharedViewModels.Product;

public class ProductDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsFeatured { get; set; }
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; }
}