using SharedViewModels.Product;
using SharedViewModels.Review;

namespace CustomerSite.Models;

public class ProductDetailModel
{
    public ProductDetailDto Product { get; set; }
    
    public ProductReviewsDto Review { get; set; }
}