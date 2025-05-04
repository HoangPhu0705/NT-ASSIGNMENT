using Application.Interfaces.Categories;
using Application.Interfaces.Products;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Validation.AspNetCore;
using SharedViewModels.Category;
using SharedViewModels.Product;
using SharedViewModels.Shared;

namespace API.Controllers;

[Route("api/admin")]
[ApiController]
[Authorize(
    AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme,
    Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly ICategoryService _categoryService;
    private readonly IProductService _productService;

    public AdminController(ICategoryService categoryService, IProductService productService)
    {
        _categoryService = categoryService;
        _productService = productService;
    }

    #region Category Operations
    
    [HttpGet("category")]
    public async Task<ActionResult<ApiResponse<IEnumerable<CategoryDto>>>> GetCategories()
    {
        try
        {
            var response = await _categoryService.GetAllCategoriesAsync();
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<string>.Error(ex.Message));
        }
    }

    [HttpGet("category/{id}")]
    public async Task<ActionResult<ApiResponse<CategoryDetailDto>>> GetCategoryById(Guid id)
    {
        try
        {
            var response = await _categoryService.GetCategoryByIdAsync(id);
            if (!response.Succeeded)
                return NotFound(response);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<string>.Error(ex.Message));
        }
    }

    [HttpPost("category")]
    public async Task<ActionResult<ApiResponse<CategoryDetailDto>>> CreateCategory([FromBody] CreateCategoryRequest request)
    {
        try
        {
            var response = await _categoryService.CreateCategoryAsync(request);
            return CreatedAtAction(nameof(GetCategoryById), new { id = response.Data.Id }, response);
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<string>.Error(ex.Message));
        }
    }

    [HttpPatch("category/{id}")]
    public async Task<ActionResult<ApiResponse<CategoryDetailDto>>> UpdateCategory(Guid id, [FromBody] UpdateCategoryRequest request)
    {
        try
        {
            var response = await _categoryService.UpdateCategoryAsync(id, request);
            if (!response.Succeeded)
                return NotFound(response);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<string>.Error(ex.Message));
        }
    }

    [HttpDelete("category/{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteCategory(Guid id)
    {
        try
        {
            var response = await _categoryService.DeleteCategoryAsync(id);
            if (!response.Succeeded)
                return NotFound(response);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<string>.Error(ex.Message));
        }
    }
    
    #endregion

    #region Product Operations
    
    [HttpGet("product")]
    public async Task<ActionResult<ApiResponse<IEnumerable<ProductDto>>>> GetProducts()
    {
        try
        {
            var response = await _productService.GetAllProductsAsync();
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<string>.Error(ex.Message));
        }
    }

    [HttpGet("product/{id}")]
    public async Task<ActionResult<ApiResponse<ProductDetailDto>>> GetProductById(Guid id)
    {
        try
        {
            var response = await _productService.GetProductByIdAsync(id);
            if (!response.Succeeded)
                return NotFound(response);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<string>.Error(ex.Message));
        }
    }

    [HttpPost("product")]
    public async Task<ActionResult<ApiResponse<ProductDetailDto>>> CreateProduct([FromBody] CreateProductRequest request)
    {
        try
        {
            var response = await _productService.CreateProductAsync(request);
            return CreatedAtAction(nameof(GetProductById), new { id = response.Data.Id }, response);
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<string>.Error(ex.Message));
        }
    }

    [HttpPatch("product/{id}")]
    public async Task<ActionResult<ApiResponse<ProductDetailDto>>> UpdateProduct(Guid id, [FromBody] UpdateProductRequest request)
    {
        try
        {
            var response = await _productService.UpdateProductAsync(id, request);
            if (!response.Succeeded)
                return NotFound(response);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<string>.Error(ex.Message));
        }
    }

    [HttpDelete("product/{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteProduct(Guid id)
    {
        try
        {
            var response = await _productService.DeleteProductAsync(id);
            if (!response.Succeeded)
                return NotFound(response);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<string>.Error(ex.Message));
        }
    }

    [HttpGet("product/category/{categoryId}")]
    public async Task<ActionResult<ApiResponse<IEnumerable<ProductDto>>>> GetProductsByCategory(Guid categoryId)
    {
        try
        {
            var response = await _productService.GetProductsByCategoryAsync(categoryId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<string>.Error(ex.Message));
        }
    }

    [HttpGet("product/search")]
    public async Task<ActionResult<ApiResponse<IEnumerable<ProductDto>>>> SearchProducts([FromQuery] string term)
    {
        try
        {
            var response = await _productService.SearchProductsAsync(term);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<string>.Error(ex.Message));
        }
    }
    
    #endregion
}