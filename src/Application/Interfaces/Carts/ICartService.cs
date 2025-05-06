using SharedViewModels.Cart;
using SharedViewModels.Shared;

namespace Application.Interfaces.Carts
{
    public interface ICartService
    {
        Task<ApiResponse<CartDto>> GetCartAsync(Guid userId);
        Task<ApiResponse<CartDto>> AddToCartAsync(Guid userId, AddToCartRequest request);
        Task<ApiResponse<CartDto>> UpdateCartItemAsync(Guid userId, UpdateCartItemRequest request);
        Task<ApiResponse<CartDto>> RemoveCartItemAsync(Guid userId, Guid itemId);
        Task<ApiResponse<bool>> ClearCartAsync(Guid userId);
    }
}