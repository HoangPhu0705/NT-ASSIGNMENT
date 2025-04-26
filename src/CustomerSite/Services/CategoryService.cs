using SharedViewModels.Category;
using SharedViewModels.Shared;

namespace CustomerSite.Services;

public class CategoryService
{
    private readonly HttpClient _httpClient;

    public CategoryService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("NextechApi");
    }
    
    public async Task<ApiResponse<List<CategoryDto>>> GetCategoriesAsync()
    {
        var response = await _httpClient.GetAsync("api/category");
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<ApiResponse<List<CategoryDto>>>();
        }
        
        var errorResponse = await response.Content.ReadFromJsonAsync<ApiResponse<List<CategoryDto>>>();
        return errorResponse ?? ApiResponse<List<CategoryDto>>.Error("Failed to fetch categories.");
    }
}