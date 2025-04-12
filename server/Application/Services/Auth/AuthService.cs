using SharedViewModels.Shared;
using Application.Interfaces.Auth;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using SharedViewModels.Auth;

namespace Application.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _config;
        private readonly IEmailSender _emailSender; 
        private readonly ITokenService _tokenService;

        public AuthService(
            UserManager<User> userManager,
            RoleManager<IdentityRole<Guid>> roleManager,
            SignInManager<User> signInManager,
            IEmailSender emailSender,
            ITokenService tokenService,
            IConfiguration config)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _tokenService = tokenService;
            _config = config;
        }

       

        public async Task<AuthResponse> HandleOAuthLoginAsync(string provider, OAuthRequest request)
        {
            var user = await _userManager.FindByLoginAsync(provider, request.ExternalId);

            if (user == null) 
            {
                user = await _userManager.FindByEmailAsync(request.Email);

                if (user == null)
                {
                    user = new User
                    {
                        Email = request.Email,
                        FirstName = request.FirstName,
                        LastName = request.LastName,
                        UserName = request.UserName,
                        ProfilePicture = request.ProfilePicture,
                        EmailConfirmed = true,
                        CreatedAt = DateTime.UtcNow,
                    };

                    var result = await _userManager.CreateAsync(user);

                    if (!result.Succeeded) 
                    {
                        throw new Exception($"Failed to create user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    }

                    await EnsureRoleExists("User");
                    await _userManager.AddToRoleAsync(user, "User");
                }

                var loginInfo = new UserLoginInfo(provider, request.ExternalId, provider);
                var addLoginResult = await _userManager.AddLoginAsync(user, loginInfo);
                if (!addLoginResult.Succeeded)
                {
                    throw new Exception($"Failed to add login: {string.Join(", ", addLoginResult.Errors.Select(e => e.Description))}");
                }
            }

            if (!user.EmailConfirmed)
            {
                throw new Exception("Email not confirmed. Please confirm your email to log in.");
            }



            return await GenerateAuthResponse(user);
        }

        public async Task<AuthResponse> LoginAsync(LoginUserRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
                throw new Exception("Invalid email or password");

            if (!user.EmailConfirmed)
                throw new Exception("Please confirm your email before logging in.");

            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
            if (!result.Succeeded)
                throw new Exception("Invalid email or password");

            return await GenerateAuthResponse(user);
        }

        public async Task<ApiResponse<string>> RegisterAsync(RegisterUserRequest request)
        {
            if(await _userManager.FindByEmailAsync(request.Email) != null)
                return ApiResponse<string>.Error("Email already in use");

            var user = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                UserName = request.FirstName + request.LastName,
                Email = request.Email,
                Dob = request.Dob,
                CreatedAt = DateTime.UtcNow,
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                throw new Exception($"User registration failed: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }

            await EnsureRoleExists("User");
            await _userManager.AddToRoleAsync(user, "User");
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            var confirmationLink = $"https://localhost:7130/api/auth/confirm-email?userId={user.Id}&token={Uri.EscapeDataString(token)}";

            await _emailSender.SendEmailAsync(user.Email, "Confirm Your Nextech Email", confirmationLink); 

            return ApiResponse<string>.Success("Registration successful. Please check your email to confirm your account.");
        }

        public async Task<ApiResponse<string>> ConfirmEmailAsync(string userId, string token)
        {
            if (!Guid.TryParse(userId, out var guidUserId))
                return ApiResponse<string>.Error("Invalid user ID");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return ApiResponse<string>.Error("User not found");

            if (user.EmailConfirmed)
                return ApiResponse<string>.Success("Email already confirmed");

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
                return ApiResponse<string>.Success("Email confirmed successfully. You can now log in.");
            else
                return ApiResponse<string>.Error($"Email confirmation failed: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }
        
        private async Task EnsureRoleExists(string roleName)
        {
            var roleExists = await _roleManager.RoleExistsAsync(roleName);
            if (!roleExists)
            {
                await _roleManager.CreateAsync(new IdentityRole<Guid> { Name = roleName });
            }
        }

        private async Task<AuthResponse> GenerateAuthResponse(User user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var token = await _tokenService.GenerateJwtTokenAsync(user, roles);
            return new AuthResponse
            {
                Token = token,
                UserId = user.Id.ToString(),
                UserName = user.UserName,
                Email = user.Email,
                Role = roles.FirstOrDefault() ?? "User"
            };
        }

    } 
   
}
