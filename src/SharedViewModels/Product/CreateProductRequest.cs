// SharedViewModels/Product/CreateProductRequest.cs

namespace SharedViewModels.Product
{
    public class CreateProductRequest
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public Guid CategoryId { get; set; }
        public bool IsParent { get; set; } = true;
        public List<CreateProductImageRequest>? Images { get; set; }  // List of images
        public List<CreateProductVariantRequest>? Variants { get; set; }
    }
    
}

