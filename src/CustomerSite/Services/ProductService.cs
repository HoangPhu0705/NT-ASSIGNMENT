using SharedViewModels.Product;
using SharedViewModels.Shared;

namespace CustomerSite.Services;

public class ProductService
{
    private readonly HttpClient _httpClient;

    public ProductService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("NextechApi");
    }
    
    public async Task<ApiResponse<List<ProductDto>>> GetProductsByCategory(Guid categoryId)
    {   
        if (string.IsNullOrEmpty(categoryId.ToString()))
        {
            return ApiResponse<List<ProductDto>>.Error("Category ID cannot be null or empty.");
        }
        
        var response = await _httpClient.GetAsync($"api/product/category/{categoryId}");
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<ApiResponse<List<ProductDto>>>();
        }
        
        var errorResponse = await response.Content.ReadFromJsonAsync<ApiResponse<List<ProductDto>>>();
        return errorResponse ?? ApiResponse<List<ProductDto>>.Error("Failed to fetch products.");
    }

    public async Task<ApiResponse<ProductDetailDto>> GetProductDetail(Guid productId)
    {
        if (string.IsNullOrEmpty(productId.ToString()))
        {
            return ApiResponse<ProductDetailDto>.Error("Product ID cannot be null or empty.");
        }
        
        var response = await _httpClient.GetAsync($"api/product/{productId}");
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<ApiResponse<ProductDetailDto>>();
        }
        
        var errorResponse = await response.Content.ReadFromJsonAsync<ApiResponse<ProductDetailDto>>();
        return errorResponse ?? ApiResponse<ProductDetailDto>.Error("Failed to fetch product details.");
    }
    

}