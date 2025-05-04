using Microsoft.AspNetCore.Mvc;
using Application.Interfaces.Products;
using SharedViewModels.Product;
using SharedViewModels.Shared;

namespace API.Controllers;

[Route("api/product")]
[ApiController]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
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

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<ProductDetailDto>>> GetProductById(Guid id)
    {
        try
        {
            var response = await _productService.GetProductByIdAsync(id);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<string>.Error(ex.Message));
        }
    }

    [HttpGet("category/{categoryId}")]
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

    [HttpGet("search")]
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
}