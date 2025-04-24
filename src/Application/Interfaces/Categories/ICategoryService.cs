using SharedViewModels.Category;
using SharedViewModels.Shared;

namespace Application.Interfaces.Categories;

public interface ICategoryService
{
    Task<ApiResponse<IEnumerable<CategoryDto>>> GetAllCategoriesAsync();
    Task<ApiResponse<CategoryDetailDto>> GetCategoryByIdAsync(Guid id);
    Task<ApiResponse<CategoryDetailDto>> CreateCategoryAsync(CreateCategoryRequest request);
    Task<ApiResponse<CategoryDetailDto>> UpdateCategoryAsync(Guid id, UpdateCategoryRequest request);
    Task<ApiResponse<bool>> DeleteCategoryAsync(Guid id);
    Task<ApiResponse<IEnumerable<CategoryDto>>> GetRootCategoriesAsync();
    Task<ApiResponse<IEnumerable<CategoryDto>>> GetSubcategoriesAsync(Guid parentId);
}