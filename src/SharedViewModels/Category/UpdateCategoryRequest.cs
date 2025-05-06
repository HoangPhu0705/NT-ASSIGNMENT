namespace SharedViewModels.Category
{
    public class UpdateCategoryRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public Guid? ParentCategoryId { get; set; }
    }

}