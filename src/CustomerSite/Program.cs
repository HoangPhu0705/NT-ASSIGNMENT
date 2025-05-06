using System.Net.Http.Headers;
using System.Text.Json;
using CustomerSite.Models;
using CustomerSite.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using OpenIddict.Abstractions;

namespace CustomerSite
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddScoped<AuthService>();
            builder.Services.AddScoped<CategoryService>();
            builder.Services.AddScoped<ProductService>();
            builder.Services.AddScoped<ReviewService>();

            builder.Services.Configure<ApiSettings>(builder.Configuration.GetSection("ApiSettings"));

            builder.Services.AddHttpClient("NextechApi", (sp, client) =>
            {
                var settings = sp.GetRequiredService<IOptions<ApiSettings>>().Value;
                client.BaseAddress = new Uri(settings.BaseUrl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            });

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
            {
                options.Authority = "https://localhost:7130";
                options.ClientId = "customer-web-client";
                options.ClientSecret = "customer-secret";
                
                options.ResponseType = OpenIdConnectResponseType.Code;
                options.SaveTokens = true;
                options.Scope.Add("openid");
                options.Scope.Add("profile");
                options.Scope.Add("email");
                options.Scope.Add("roles");
                options.Scope.Add("api");
                options.Scope.Add("offline_access");
                options.CallbackPath = "/signin-oidc";
                options.SignedOutCallbackPath = "/signout-callback-oidc";

                options.Events = new OpenIdConnectEvents
                {
                    OnRemoteFailure = context =>
                    {
                        context.HandleResponse();
                        context.Response.Redirect("/Home/Error?message=" + 
                                                  Uri.EscapeDataString(context.Failure.Message));
                        return Task.CompletedTask;
                    },
                    OnAuthorizationCodeReceived = context =>
                    {
                        var logger = context.HttpContext.RequestServices
                            .GetRequiredService<ILogger<Program>>();
                        logger.LogInformation("Authorization code received");
                        return Task.CompletedTask;
                    },
                    OnTokenResponseReceived = context =>
                    {   
                        // Console.WriteLine("Access tokens: " + context.TokenEndpointResponse.AccessToken);
                        // Console.WriteLine("Refresh tokens: " + context.TokenEndpointResponse.RefreshToken);
                        return Task.CompletedTask;
                    },
                };

                options.GetClaimsFromUserInfoEndpoint = true;

                // // Map the claims from token to user identity
                options.ClaimActions.MapJsonKey("first_name", "first_name");
                options.ClaimActions.MapJsonKey("last_name", "last_name");
                options.ClaimActions.MapJsonKey("email", "email");
                options.ClaimActions.MapUniqueJsonKey("role", "role");
                
                // Set standard name and role claim types
                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    NameClaimType = "name",
                    RoleClaimType = "role"
                };

            });

            builder.Services.AddLogging(logging =>
            {
                logging.AddConsole();
                logging.SetMinimumLevel(LogLevel.Debug);
            });

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}