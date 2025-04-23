namespace SharedViewModels.Category;

public class CreateCategoryRequest
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string ImageUrl { get; set; }
    public Guid? ParentCategoryId { get; set; }
}