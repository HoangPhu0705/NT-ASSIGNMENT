// SharedViewModels/Product/ProductDto.cs
using System;

namespace SharedViewModels.Product;

public class ProductDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public List<ProductImageDto> Images { get; set; } = new();
    public string? MainImageUrl => Images?.FirstOrDefault(i => i.IsPrimary)?.ImageUrl;
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; }
}