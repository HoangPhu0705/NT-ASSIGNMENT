using Application.Interfaces.Categories;
using AutoMapper;
using Domain.Entities;
using SharedViewModels.Category;
using SharedViewModels.Shared;

namespace Application.Services.Categories;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;

    public CategoryService(ICategoryRepository categoryRepository, IMapper mapper)
    {
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }
    
    public async Task<ApiResponse<IEnumerable<CategoryDto>>> GetAllCategoriesAsync()
    {
        var categories = await _categoryRepository.GetAllAsync();
        var categoriesDto = _mapper.Map<IEnumerable<CategoryDto>>(categories);
        return ApiResponse<IEnumerable<CategoryDto>>.Success(categoriesDto);
    }
    
    public async Task<ApiResponse<IEnumerable<CategoryAttributeDto>>> GetCategoryAttributesAsync(Guid categoryId)
    {
        var exists = await _categoryRepository.ExistsAsync(categoryId);
        if (!exists)
            return ApiResponse<IEnumerable<CategoryAttributeDto>>.Error("Category not found");

        var attributes = await _categoryRepository.GetCategoryAttributesAsync(categoryId);
        var filteredAttributes = attributes.Where(a => a.IsFilterable).ToList();
        var attributesDto = _mapper.Map<IEnumerable<CategoryAttributeDto>>(filteredAttributes);
        return ApiResponse<IEnumerable<CategoryAttributeDto>>.Success(attributesDto);
    }
    
    public async Task<ApiResponse<CategoryDetailDto>> GetCategoryByIdAsync(Guid id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
                return ApiResponse<CategoryDetailDto>.Error("Category not found");

            var categoryDto = _mapper.Map<CategoryDetailDto>(category);
            return ApiResponse<CategoryDetailDto>.Success(categoryDto);
        }

        public async Task<ApiResponse<CategoryDetailDto>> CreateCategoryAsync(CreateCategoryRequest request)
        {
            // Validate parent category if specified
            if (request.ParentCategoryId.HasValue)
            {
                var parentExists = await _categoryRepository.ExistsAsync(request.ParentCategoryId.Value);
                if (!parentExists)
                    return ApiResponse<CategoryDetailDto>.Error("Parent category not found");
            }

            var category = new Category
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Description = request.Description,
                ImageUrl = request.ImageUrl,
                ParentCategoryId = request.ParentCategoryId
            };

            var createdCategory = await _categoryRepository.AddAsync(category);
            var categoryWithDetails = await _categoryRepository.GetByIdAsync(createdCategory.Id);
            var categoryDto = _mapper.Map<CategoryDetailDto>(categoryWithDetails);

            return ApiResponse<CategoryDetailDto>.Created(categoryDto);
        }

        public async Task<ApiResponse<CategoryDetailDto>> UpdateCategoryAsync(Guid id, UpdateCategoryRequest request)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
                return ApiResponse<CategoryDetailDto>.Error("Category not found");

            // Prevent circular references
            if (request.ParentCategoryId == id)
                return ApiResponse<CategoryDetailDto>.Error("A category cannot be its own parent");

            // Validate parent category if specified
            if (request.ParentCategoryId.HasValue)
            {
                var parentExists = await _categoryRepository.ExistsAsync(request.ParentCategoryId.Value);
                if (!parentExists)
                    return ApiResponse<CategoryDetailDto>.Error("Parent category not found");
            }

            category.Name = request.Name;
            category.Description = request.Description;
            category.ImageUrl = request.ImageUrl;
            category.ParentCategoryId = request.ParentCategoryId;

            await _categoryRepository.UpdateAsync(category);
            
            var updatedCategory = await _categoryRepository.GetByIdAsync(id);
            var categoryDto = _mapper.Map<CategoryDetailDto>(updatedCategory);

            return ApiResponse<CategoryDetailDto>.Success(categoryDto);
        }

        public async Task<ApiResponse<bool>> DeleteCategoryAsync(Guid id)
        {
            var exists = await _categoryRepository.ExistsAsync(id);
            if (!exists)
                return ApiResponse<bool>.Error("Category not found");

            // Check for subcategories
            var hasSubcategories = await _categoryRepository.HasSubcategoriesAsync(id);
            if (hasSubcategories)
                return ApiResponse<bool>.Error("Cannot delete category with subcategories");

            // Check for products
            var hasProducts = await _categoryRepository.HasProductsAsync(id);
            if (hasProducts)
                return ApiResponse<bool>.Error("Cannot delete category with associated products");

            var result = await _categoryRepository.DeleteAsync(id);
            return ApiResponse<bool>.Success(result);
        }

        public async Task<ApiResponse<IEnumerable<CategoryDto>>> GetRootCategoriesAsync()
        {
            var categories = await _categoryRepository.GetRootCategoriesAsync();
            var categoriesDto = _mapper.Map<IEnumerable<CategoryDto>>(categories);
            return ApiResponse<IEnumerable<CategoryDto>>.Success(categoriesDto);
        }

        public async Task<ApiResponse<IEnumerable<CategoryDto>>> GetSubcategoriesAsync(Guid parentId)
        {
            var exists = await _categoryRepository.ExistsAsync(parentId);
            if (!exists)
                return ApiResponse<IEnumerable<CategoryDto>>.Error("Parent category not found");

            var categories = await _categoryRepository.GetSubcategoriesAsync(parentId);
            var categoriesDto = _mapper.Map<IEnumerable<CategoryDto>>(categories);
            return ApiResponse<IEnumerable<CategoryDto>>.Success(categoriesDto);
        }
    
}