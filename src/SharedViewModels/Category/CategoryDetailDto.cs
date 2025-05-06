

namespace SharedViewModels.Category
{
    public class CategoryDetailDto : CategoryDto
    {
        public IEnumerable<CategoryDto> SubCategories { get; set; } = new List<CategoryDto>();
    }
}
