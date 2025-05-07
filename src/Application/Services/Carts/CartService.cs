using Application.Interfaces.Carts;
using AutoMapper;
using Domain.Entities;
using SharedViewModels.Cart;
using SharedViewModels.Shared;

namespace Application.Services.Carts
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IMapper _mapper;

        public CartService(ICartRepository cartRepository, IMapper mapper)
        {
            _cartRepository = cartRepository;
            _mapper = mapper;
        }

        public async Task<ApiResponse<CartDto>> GetCartAsync(Guid userId)
        {
            var cartItems = await _cartRepository.GetCartItemsByUserIdAsync(userId);
            var cartItemDtos = _mapper.Map<List<CartItemDto>>(cartItems);

            var cartDto = new CartDto
            {
                Id = Guid.NewGuid(), // Virtual cart ID
                Items = cartItemDtos,
            };

            return ApiResponse<CartDto>.Success(cartDto);
        }

        public async Task<ApiResponse<CartDto>> AddToCartAsync(Guid userId, AddToCartRequest request)
        {
            // Check if this item already exists in the cart
            var existingItem = await _cartRepository.GetCartItemAsync(userId, request.ProductId, request.VariantId);

            if (existingItem != null)
            {
                // Update quantity of existing item
                existingItem.Quantity += request.Quantity;
                await _cartRepository.UpdateCartItemAsync(existingItem);
            }
            else
            {
                // Add new cart item
                var newCartItem = new Cart
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    ProductId = request.ProductId,
                    ProductVariantId = request.VariantId,
                    Quantity = request.Quantity
                };

                await _cartRepository.AddCartItemAsync(newCartItem);
            }

            // Return the updated cart
            return await GetCartAsync(userId);
        }

        public async Task<ApiResponse<CartDto>> UpdateCartItemAsync(Guid userId, UpdateCartItemRequest request)
        {
            var cartItem = await _cartRepository.GetCartItemByIdAsync(request.CartItemId);

            if (cartItem == null || cartItem.UserId != userId)
                return ApiResponse<CartDto>.Error("Cart item not found");

            if (request.Quantity <= 0)
            {
                await _cartRepository.DeleteCartItemAsync(request.CartItemId);
            }
            else
            {
                cartItem.Quantity = request.Quantity;
                await _cartRepository.UpdateCartItemAsync(cartItem);
            }

            // Return the updated cart
            return await GetCartAsync(userId);
        }

        public async Task<ApiResponse<CartDto>> RemoveCartItemAsync(Guid userId, Guid itemId)
        {
            var cartItem = await _cartRepository.GetCartItemByIdAsync(itemId);

            if (cartItem == null || cartItem.UserId != userId)
                return ApiResponse<CartDto>.Error("Cart item not found");

            await _cartRepository.DeleteCartItemAsync(itemId);

            return await GetCartAsync(userId);
        }

        public async Task<ApiResponse<bool>> ClearCartAsync(Guid userId)
        {
            var result = await _cartRepository.ClearCartAsync(userId);
            return ApiResponse<bool>.Success(result);
        }
    }
}