using SharedViewModels.Review;
using SharedViewModels.Shared;

namespace CustomerSite.Services;

public class ReviewService
{
    private readonly HttpClient _httpClient;
    
    public ReviewService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("NextechApi");    
    }
    
    public async Task<ApiResponse<ProductReviewsDto>> GetReviewsByProductId(Guid productId)
    {
        var response = await _httpClient.GetAsync($"api/review/product/{productId}");
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<ApiResponse<ProductReviewsDto>>();
        }
        string errorMessage = "Failed to fetch reviews.";
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

        return ApiResponse<ProductReviewsDto>.Error(errorMessage);
    }
    
    //Create reviews 
    public async Task<ApiResponse<ReviewDto>> CreateReview(CreateReviewRequest request, string accessToken)
    {
        _httpClient.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
    
        var response = await _httpClient.PostAsJsonAsync("api/review", request);
    
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<ApiResponse<ReviewDto>>();
        }
    
        string errorMessage = "Failed to create review.";
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

        return ApiResponse<ReviewDto>.Error(errorMessage);
    }

}