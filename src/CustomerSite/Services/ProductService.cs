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
    
    public async Task<ApiResponse<List<ProductDto>>> GetProductsByCategory(string categoryId)
    {   
        if (string.IsNullOrEmpty(categoryId))
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

}