using SharedViewModels.Auth;
using SharedViewModels.Shared;

namespace CustomerSite.Services;

public class AuthService
{
    private readonly HttpClient _httpClient;

    public AuthService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("NextechApi");
    }
    
    public async Task<ApiResponse<string>> RegisterAsync(RegisterUserRequest request)
    {
        
        var response = await _httpClient.PostAsJsonAsync("api/auth/register", request);
        
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<ApiResponse<string>>();
        }
        
        var errorResponse = await response.Content.ReadFromJsonAsync<ApiResponse<string>>();
        return ApiResponse<string>.Error(errorResponse?.Message ?? "An error occurred while processing your request.");
    }
    public async Task<ApiResponse<string>> ResendConfirmationEmailAsync(string email)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/resend-confirmation-email", email);

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<ApiResponse<string>>();
        }

        var errorResponse = await response.Content.ReadFromJsonAsync<ApiResponse<string>>();
        return errorResponse ?? ApiResponse<string>.Error("Failed to resend confirmation email.");
    }
}