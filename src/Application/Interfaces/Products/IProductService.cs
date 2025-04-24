using SharedViewModels.Product;
using SharedViewModels.Shared;

namespace Application.Interfaces.Products;

public interface IProductService
{
    Task<ApiResponse<IEnumerable<ProductDto>>> GetAllProductsAsync();
    Task<ApiResponse<ProductDetailDto>> GetProductByIdAsync(Guid id);
    Task<ApiResponse<ProductDetailDto>> CreateProductAsync(CreateProductRequest request);
    Task<ApiResponse<ProductDetailDto>> UpdateProductAsync(Guid id, UpdateProductRequest request);
    Task<ApiResponse<bool>> DeleteProductAsync(Guid id);
    Task<ApiResponse<IEnumerable<ProductDto>>> GetProductsByCategoryAsync(Guid categoryId);
    Task<ApiResponse<IEnumerable<ProductDto>>> SearchProductsAsync(string searchTerm);
}