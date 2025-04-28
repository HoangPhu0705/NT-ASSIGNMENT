using SharedViewModels.Category;
using SharedViewModels.Product;

namespace CustomerSite.Models;

public class ShopViewModel
{
    public CategoryDto Category { get; set; }
    public List<ProductDto> Products { get; set; }
}