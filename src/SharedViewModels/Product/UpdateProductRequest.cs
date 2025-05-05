namespace SharedViewModels.Product;

public class UpdateProductRequest
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal? Price { get; set; }
    public int? Stock { get; set; }
    public bool? IsFeatured { get; set; }
    public Guid? CategoryId { get; set; }
    public List<UpdateProductImageRequest>? Images { get; set; }
    public List<UpdateProductVariantRequest>? Variants { get; set; }
    public bool? IsParent { get; set; }
}