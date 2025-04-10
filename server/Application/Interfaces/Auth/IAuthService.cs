using Application.DTOs.Auth;
using Application.DTOs.Shared;

namespace Application.Interfaces.Auth
{
    public interface IAuthService
    {
        Task<ApiResponse<string>> RegisterAsync(RegisterUserRequest request);

        Task<AuthResponse> LoginAsync(LoginUserRequest request);

        Task<AuthResponse> HandleOAuthLoginAsync(string provider, OAuthRequest request);

        Task<ApiResponse<string>> ConfirmEmailAsync(string userId, string token);
    }
}
        