using Domain.Entities;
using SharedViewModels.Shared;
using SharedViewModels.Auth;

namespace Application.Interfaces.Auth
{
    public interface IAuthService
    {
        Task<ApiResponse<string>> RegisterAsync(RegisterUserRequest request);
        Task<ApiResponse<string>> ConfirmEmailAsync(string userId, string token);
        Task<User> CreateOrGetUserFromOAuthAsync(string provider, OAuthRequest request);
    }
}
        