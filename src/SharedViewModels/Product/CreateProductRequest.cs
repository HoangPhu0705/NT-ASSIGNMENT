// SharedViewModels/Product/CreateProductRequest.cs
using System;

namespace SharedViewModels.Product;

public class CreateProductRequest
{
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public string? ImageUrl { get; set; }
    public Guid CategoryId { get; set; }
}