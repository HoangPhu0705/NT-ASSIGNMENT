using Domain.Entities;

namespace Application.Interfaces.Carts
{
    public interface ICartRepository
    {
        Task<IEnumerable<Cart>> GetCartItemsByUserIdAsync(Guid userId);
        Task<Cart> GetCartItemByIdAsync(Guid id);
        Task<Cart> GetCartItemAsync(Guid userId, Guid productId, Guid variantId);
        Task<Cart> AddCartItemAsync(Cart cartItem);
        Task UpdateCartItemAsync(Cart cartItem);
        Task<bool> DeleteCartItemAsync(Guid id);
        Task<bool> ClearCartAsync(Guid userId);
    }
}