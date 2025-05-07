using SharedViewModels.Cart;
using SharedViewModels.Shared;

namespace CustomerSite.Services;

public class CartService
{
    private readonly HttpClient _httpClient;
    
    public CartService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("NextechApi");    
    }
    
    public async Task<ApiResponse<CartDto>> GetCartAsync(string accessToken)
    {
        _httpClient.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
        
        var response = await _httpClient.GetAsync("api/cart");
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<ApiResponse<CartDto>>();
        }
        
        string errorMessage = "Failed to fetch cart.";
        try
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            if (!string.IsNullOrEmpty(errorContent))
            {
                errorMessage = errorContent;
            }
        }
        catch
        {
        }

        return ApiResponse<CartDto>.Error(errorMessage);
    }
    
    public async Task<ApiResponse<CartDto>> AddToCartAsync(AddToCartRequest request, string accessToken)
    {
        _httpClient.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
        
        var response = await _httpClient.PostAsJsonAsync("api/cart", request);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<ApiResponse<CartDto>>();
        }
        
        string errorMessage = "Failed to add item to cart.";
        try
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            if (!string.IsNullOrEmpty(errorContent))
            {
                errorMessage = errorContent;
            }
        }
        catch
        {
        }

        return ApiResponse<CartDto>.Error(errorMessage);
    }
    
    public async Task<ApiResponse<CartDto>> UpdateCartItemAsync(UpdateCartItemRequest request, string accessToken)
    {
        _httpClient.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
        
        var response = await _httpClient.PutAsJsonAsync("api/cart", request);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<ApiResponse<CartDto>>();
        }
        
        string errorMessage = "Failed to update cart item.";
        try
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            if (!string.IsNullOrEmpty(errorContent))
            {
                errorMessage = errorContent;
            }
        }
        catch
        {
        }

        return ApiResponse<CartDto>.Error(errorMessage);
    }
    
    public async Task<ApiResponse<CartDto>> RemoveCartItemAsync(Guid itemId, string accessToken)
    {
        _httpClient.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
        
        var response = await _httpClient.DeleteAsync($"api/cart/{itemId}");
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<ApiResponse<CartDto>>();
        }
        
        string errorMessage = "Failed to remove item from cart.";
        try
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            if (!string.IsNullOrEmpty(errorContent))
            {
                errorMessage = errorContent;
            }
        }
        catch
        {
        }

        return ApiResponse<CartDto>.Error(errorMessage);
    }
    
}