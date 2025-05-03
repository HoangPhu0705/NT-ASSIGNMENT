using Application.Interfaces.Categories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Validation.AspNetCore;
using SharedViewModels.Category;
using SharedViewModels.Shared;

namespace API.Controllers;

[Route("api/admin/category")]
[ApiController]
[Authorize(
    AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme, 
    Roles = "Admin")]
public class AdminController : ControllerBase
{
    private ICategoryService _categoryService;

    public AdminController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }
    
    [HttpGet]
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
    
    [HttpGet("{id}")]
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

    // Create operation - Admin only
    [HttpPost]
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

    // Update operation - Admin only
    [HttpPatch("{id}")]
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

    // Delete operation - Admin only
    [HttpDelete("{id}")]
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
    
}