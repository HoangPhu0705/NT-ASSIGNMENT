using Application.Interfaces.Reviews;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly AppDbContext _context;

        public ReviewRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProductReview>> GetByProductIdAsync(Guid productId)
        {
            return await _context.ProductReviews
                .Include(r => r.User)
                .Where(r => r.ProductId == productId && r.IsApproved)
                .OrderByDescending(r => r.ReviewDate)
                .ToListAsync();
        }

        public async Task<ProductReview> GetByIdAsync(Guid reviewId)
        {
            return await _context.ProductReviews
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Id == reviewId);
        }

        public async Task<IEnumerable<ProductReview>> GetByUserIdAsync(Guid userId)
        {
            return await _context.ProductReviews
                .Include(r => r.Product)
                .Where(r => r.UserId == userId)
                .ToListAsync();
        }

        public async Task<ProductReview> AddAsync(ProductReview review)
        {
            _context.ProductReviews.Add(review);
            await _context.SaveChangesAsync();
            return review;
        }

        public async Task UpdateAsync(ProductReview review)
        {
            _context.Entry(review).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(Guid reviewId)
        {
            var review = await _context.ProductReviews.FindAsync(reviewId);
            if (review == null)
                return false;

            _context.ProductReviews.Remove(review);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(Guid reviewId)
        {
            return await _context.ProductReviews.AnyAsync(r => r.Id == reviewId);
        }

        public async Task<bool> UserHasReviewedProductAsync(Guid userId, Guid productId)
        {
            return await _context.ProductReviews.AnyAsync(r => r.UserId == userId && r.ProductId == productId);
        }
    }
}