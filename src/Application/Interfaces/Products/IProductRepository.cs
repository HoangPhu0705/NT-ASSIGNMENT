using Domain.Entities;
using SharedViewModels.Product;

namespace Application.Interfaces.Products;

public interface IProductRepository
{
    Task<IEnumerable<Product>> GetAllAsync();
    Task<Product> GetByIdAsync(Guid id);
    Task<Product> AddAsync(Product product);
    Task UpdateAsync(Product product);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
    Task<IEnumerable<Product>> GetByCategoryAsync(Guid categoryId);
    Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm);
    Task DeleteProductImagesAsync(IEnumerable<Guid> imageIds);
    Task DeleteProductVariantsAsync(IEnumerable<Guid> variantIds);
    Task UpdateVariantAttributesAsync(ProductVariant variant, IEnumerable<UpdateVariantAttributeRequest> attributes);
}