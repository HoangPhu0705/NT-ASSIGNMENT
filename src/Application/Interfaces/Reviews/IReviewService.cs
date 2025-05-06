using SharedViewModels.Review;
using SharedViewModels.Shared;

namespace Application.Interfaces.Reviews
{
    public interface IReviewService
    {
        Task<ApiResponse<ProductReviewsDto>> GetProductReviewsAsync(Guid productId);
        Task<ApiResponse<IEnumerable<ReviewDto>>> GetUserReviewsAsync(Guid userId);
        Task<ApiResponse<ReviewDto>> CreateReviewAsync(Guid userId, CreateReviewRequest request);
        Task<ApiResponse<ReviewDto>> UpdateReviewAsync(Guid userId, UpdateReviewRequest request);
        Task<ApiResponse<bool>> DeleteReviewAsync(Guid userId, Guid reviewId);
        Task<ApiResponse<bool>> UserHasReviewedProductAsync(Guid userId, Guid productId);
    }
}