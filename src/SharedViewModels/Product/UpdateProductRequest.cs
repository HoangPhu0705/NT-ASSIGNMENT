// SharedViewModels/Product/UpdateProductRequest.cs
using System;

namespace SharedViewModels.Product;

public class UpdateProductRequest
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal? Price { get; set; }
    public int? StockQuantity { get; set; }
    public bool? IsFeatured { get; set; }
    public Guid? CategoryId { get; set; }
    public List<UpdateProductImageRequest>? Images { get; set; }
}