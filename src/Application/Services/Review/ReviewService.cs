using Application.Interfaces.Reviews;
using AutoMapper;
using Domain.Entities;
using SharedViewModels.Review;
using SharedViewModels.Shared;

namespace Application.Services.Review
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IMapper _mapper;

        public ReviewService(IReviewRepository reviewRepository, IMapper mapper)
        {
            _reviewRepository = reviewRepository;
            _mapper = mapper;
        }

        public async Task<ApiResponse<ProductReviewsDto>> GetProductReviewsAsync(Guid productId)
        {
            var reviews = await _reviewRepository.GetByProductIdAsync(productId);
            var reviewDtos = _mapper.Map<List<ReviewDto>>(reviews);

            double averageRating = 0;
            if (reviewDtos.Any())
            {
                averageRating = reviewDtos.Average(r => r.Rating);
            }

            var ratingCounts = new Dictionary<string, int>
            {
                { "1star", reviewDtos.Count(r => r.Rating == 1) },
                { "2star", reviewDtos.Count(r => r.Rating == 2) },
                { "3star", reviewDtos.Count(r => r.Rating == 3) },
                { "4star", reviewDtos.Count(r => r.Rating == 4) },
                { "5star", reviewDtos.Count(r => r.Rating == 5) }
            };

            var productReviewsDto = new ProductReviewsDto
            {
                ProductId = productId,
                AverageRating = Math.Round(averageRating, 1),
                ReviewCount = reviewDtos.Count,
                RatingCounts = ratingCounts,
                Reviews = reviewDtos
            };

            return ApiResponse<ProductReviewsDto>.Success(productReviewsDto);
        }

        public async Task<ApiResponse<IEnumerable<ReviewDto>>> GetUserReviewsAsync(Guid userId)
        {
            var reviews = await _reviewRepository.GetByUserIdAsync(userId);
            var reviewDtos = _mapper.Map<IEnumerable<ReviewDto>>(reviews);
            return ApiResponse<IEnumerable<ReviewDto>>.Success(reviewDtos);
        }

        public async Task<ApiResponse<ReviewDto>> CreateReviewAsync(Guid userId, CreateReviewRequest request)
        {
            var hasReviewed = await _reviewRepository.UserHasReviewedProductAsync(userId, request.ProductId);
            if (hasReviewed)
                return ApiResponse<ReviewDto>.Error("You have already reviewed this product");

            if (request.Rating < 1 || request.Rating > 5)
                return ApiResponse<ReviewDto>.Error("Rating must be between 1 and 5");

            var review = new ProductReview
            {
                Id = Guid.NewGuid(),
                ProductId = request.ProductId,
                UserId = userId,
                Rating = request.Rating,
                ReviewText = request.ReviewText,
                ReviewDate = DateTime.UtcNow,
                IsApproved = true // You might want to change this based on your moderation policy
            };

            var createdReview = await _reviewRepository.AddAsync(review);
            var reviewDto = _mapper.Map<ReviewDto>(createdReview);
            
            return ApiResponse<ReviewDto>.Created(reviewDto);
        }

        public async Task<ApiResponse<ReviewDto>> UpdateReviewAsync(Guid userId, UpdateReviewRequest request)
        {
            var review = await _reviewRepository.GetByIdAsync(request.ReviewId);
            if (review == null)
                return ApiResponse<ReviewDto>.Error("Review not found");

            if (review.UserId != userId)
                return ApiResponse<ReviewDto>.Error("You can only update your own reviews");

            if (request.Rating < 1 || request.Rating > 5)
                return ApiResponse<ReviewDto>.Error("Rating must be between 1 and 5");

            review.Rating = request.Rating;
            review.ReviewText = request.ReviewText;
            
            await _reviewRepository.UpdateAsync(review);
            
            var reviewDto = _mapper.Map<ReviewDto>(review);
            return ApiResponse<ReviewDto>.Success(reviewDto);
        }

        public async Task<ApiResponse<bool>> DeleteReviewAsync(Guid userId, Guid reviewId)
        {
            var review = await _reviewRepository.GetByIdAsync(reviewId);
            if (review == null)
                return ApiResponse<bool>.Error("Review not found");

            if (review.UserId != userId)
                return ApiResponse<bool>.Error("You can only delete your own reviews");

            var result = await _reviewRepository.DeleteAsync(reviewId);
            return ApiResponse<bool>.Success(result);
        }

        public async Task<ApiResponse<bool>> UserHasReviewedProductAsync(Guid userId, Guid productId)
        {
            var result = await _reviewRepository.UserHasReviewedProductAsync(userId, productId);
            return ApiResponse<bool>.Success(result);
        }
    }
}