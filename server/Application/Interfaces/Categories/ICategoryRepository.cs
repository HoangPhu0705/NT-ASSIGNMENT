using Domain.Entities;

namespace Application.Interfaces.Categories;

public interface ICategoryRepository
{
    Task<IEnumerable<Category>> GetAllAsync();
    Task<Category> GetByIdAsync(Guid id);
    Task<Category> AddAsync(Category category);
    Task UpdateAsync(Category category);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
    Task<bool> HasSubcategoriesAsync(Guid id);
    Task<bool> HasProductsAsync(Guid id);
    Task<IEnumerable<Category>> GetRootCategoriesAsync();
    Task<IEnumerable<Category>> GetSubcategoriesAsync(Guid parentId);
}