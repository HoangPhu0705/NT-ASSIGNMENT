namespace SharedViewModels.Product;

public class UpdateProductImageRequest
{
    public Guid? Id { get; set; }  // For existing images
    public string ImageUrl { get; set; }
    public bool IsMain { get; set; }
    public bool IsDeleted { get; set; }  // To mark images for deletion
}