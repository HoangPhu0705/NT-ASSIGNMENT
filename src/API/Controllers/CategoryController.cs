using Microsoft.AspNetCore.Mvc;
using Application.Interfaces.Categories;
using SharedViewModels.Category;
using SharedViewModels.Shared;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Controllers;

[Route("api/category")]
[ApiController]
public class CategoryController : ControllerBase
{
    private readonly ICategoryService _categoryService;
    public CategoryController(ICategoryService categoryService)
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
    
    [HttpGet("{id}/filters")]
    public async Task<ActionResult<ApiResponse<IEnumerable<CategoryAttributeDto>>>> GetCategoryFilters(Guid id)
    {
        try
        {
            var response = await _categoryService.GetCategoryAttributesAsync(id);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<string>.Error(ex.Message));
        }
    }
    
    [HttpGet("root")]
    public async Task<ActionResult<ApiResponse<IEnumerable<CategoryDto>>>> GetRootCategories()
    {
        try
        {
            var response = await _categoryService.GetRootCategoriesAsync();
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
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<string>.Error(ex.Message));
        }
    }
    
    [HttpGet("{id}/subcategories")]
    public async Task<ActionResult<ApiResponse<IEnumerable<CategoryDto>>>> GetSubcategories(Guid id)
    {
        try
        {
            var response = await _categoryService.GetSubcategoriesAsync(id);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<string>.Error(ex.Message));
        }
    }
    
}