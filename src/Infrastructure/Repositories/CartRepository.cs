using Application.Interfaces.Carts;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly AppDbContext _context;

        public CartRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Cart>> GetCartItemsByUserIdAsync(Guid userId)
        {
            return await _context.Cart
                .Include(c => c.Product)
                    .ThenInclude(p => p.Images)
                .Include(c => c.ProductVariant)
                .Where(c => c.UserId == userId)
                .ToListAsync();
        }

        public async Task<Cart> GetCartItemByIdAsync(Guid id)
        {
            return await _context.Cart
                .Include(c => c.Product)
                .Include(c => c.ProductVariant)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Cart> GetCartItemAsync(Guid userId, Guid productId, Guid variantId)
        {
            return await _context.Cart
                .FirstOrDefaultAsync(c => c.UserId == userId && 
                                     c.ProductId == productId && 
                                     c.ProductVariantId == variantId);
        }

        public async Task<Cart> AddCartItemAsync(Cart cartItem)
        {
            _context.Cart.Add(cartItem);
            await _context.SaveChangesAsync();
            return cartItem;
        }

        public async Task UpdateCartItemAsync(Cart cartItem)
        {
            _context.Entry(cartItem).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteCartItemAsync(Guid id)
        {
            var cartItem = await _context.Cart.FindAsync(id);
            if (cartItem == null)
                return false;

            _context.Cart.Remove(cartItem);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ClearCartAsync(Guid userId)
        {
            var cartItems = await _context.Cart
                .Where(c => c.UserId == userId)
                .ToListAsync();

            if (!cartItems.Any())
                return false;

            _context.Cart.RemoveRange(cartItems);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}