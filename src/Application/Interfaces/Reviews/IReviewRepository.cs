using Domain.Entities;

namespace Application.Interfaces.Reviews
{
    public interface IReviewRepository
    {
        Task<IEnumerable<ProductReview>> GetByProductIdAsync(Guid productId);
        Task<ProductReview> GetByIdAsync(Guid reviewId);
        Task<IEnumerable<ProductReview>> GetByUserIdAsync(Guid userId);
        Task<ProductReview> AddAsync(ProductReview review);
        Task UpdateAsync(ProductReview review);
        Task<bool> DeleteAsync(Guid reviewId);
        Task<bool> ExistsAsync(Guid reviewId);
        Task<bool> UserHasReviewedProductAsync(Guid userId, Guid productId);
    }
}