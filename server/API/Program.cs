using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OpenIddict.Abstractions;
using Quartz;
using System.Security.Cryptography.X509Certificates;
using Application.Interfaces.Auth;
using Application.Services.Auth;
using Domain.Entities;
using DotNetEnv;
using Infrastructure.Data;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity.UI.Services;
using OpenIddict.Server;
using OpenIddict.Server.AspNetCore;

namespace API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Env.Load();
            var builder = WebApplication.CreateBuilder(args);

            // Register DBContext
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Register Identity
            builder.Services.AddIdentity<User, IdentityRole<Guid>>(options =>
                {
                    options.SignIn.RequireConfirmedAccount = true;
                })
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            // Configure Identity 
            builder.Services.Configure<IdentityOptions>(options =>
            {
                options.ClaimsIdentity.UserNameClaimType = OpenIddictConstants.Claims.Name;
                options.ClaimsIdentity.UserIdClaimType = OpenIddictConstants.Claims.Subject;
                options.ClaimsIdentity.RoleClaimType = OpenIddictConstants.Claims.Role;
                options.ClaimsIdentity.EmailClaimType = OpenIddictConstants.Claims.Email;
            });
            
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie(options =>
            {
                options.Cookie.Name = "nextech_auth";
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.SameSite = SameSiteMode.Strict;
                options.ExpireTimeSpan = TimeSpan.FromHours(1);
                options.SlidingExpiration = true;
            });

            // OpenIddict
            builder.Services.AddOpenIddict()
            .AddCore(options =>
            {
                options.UseEntityFrameworkCore()
                       .UseDbContext<AppDbContext>();
                // token cleanup
                options.UseQuartz();
            }).AddServer(options =>
            {
                options.AllowAuthorizationCodeFlow()
                       .AllowPasswordFlow()
                       .AllowRefreshTokenFlow()
                       .AllowClientCredentialsFlow();

                options.SetTokenEndpointUris("/connect/token")
                    .SetAuthorizationEndpointUris("/connect/authorize")
                    .SetUserInfoEndpointUris("/connect/userinfo")
                    .SetEndSessionEndpointUris("/connect/logout");

                options.AddDevelopmentEncryptionCertificate()
                       .AddDevelopmentSigningCertificate();

                options.RegisterScopes("email","profile","roles","offline_access","api");

                options.AcceptAnonymousClients();

                // Register the signing and encryption credentials
                options.DisableAccessTokenEncryption();

                options.UseAspNetCore()
                       .EnableTokenEndpointPassthrough()
                       .EnableAuthorizationEndpointPassthrough()
                       .EnableUserInfoEndpointPassthrough()
                       .EnableEndSessionEndpointPassthrough();
                
                options.SetAccessTokenLifetime(TimeSpan.FromHours(1));
                options.SetRefreshTokenLifetime(TimeSpan.FromDays(14));

                options.AddEventHandler<OpenIddictServerEvents.GenerateTokenContext>(builder =>
                {
                    builder.UseInlineHandler(context =>
                    {
                        var httpContext = context.Transaction.GetHttpRequest()?.HttpContext;
                        // Get the access token and refresh token
                        if (httpContext == null)
                        {
                            Console.WriteLine("HttpContext is null in GenerateTokenContext");
                            return default;
                        }
                        var tokenType = context.TokenType;
                        var token = context.Token;

                        if (string.IsNullOrEmpty(token)) return default;
                        switch (tokenType)
                        {
                            case "access_token":
                            {
                                var accessTokenCookieOptions = new CookieOptions
                                {
                                    HttpOnly = true,
                                    Secure = true,
                                    SameSite = SameSiteMode.Strict,
                                    Expires = DateTimeOffset.UtcNow.AddHours(1)
                                };
                                httpContext.Response.Cookies.Append("access_token", token, accessTokenCookieOptions);
                                break;
                            }
                            case "refresh_token":
                            {
                                var refreshTokenCookieOptions = new CookieOptions
                                {
                                    HttpOnly = true,
                                    Secure = true,
                                    SameSite = SameSiteMode.Strict,
                                    Expires = DateTimeOffset.UtcNow.AddDays(14)
                                };
                                httpContext.Response.Cookies.Append("refresh_token", token, refreshTokenCookieOptions);
                                break;
                            }
                        }
                        return default;
                    });
                });

            }).AddValidation(options =>
            {
                options.UseLocalServer();
                options.UseAspNetCore();
            });

            // Register Quartz.NET for token cleanup
            builder.Services.AddQuartz(options =>
            {
                options.UseMicrosoftDependencyInjectionJobFactory();
                options.UseSimpleTypeLoader();
                options.UseInMemoryStore();
            });
            
            builder.Services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

            // Register Services
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IEmailSender, EmailSender>();
            builder.Services.AddScoped<DbSeeder>();

            // Configure OAuth providers
            builder.Services.AddAuthentication()
                .AddGoogle(options =>
                {
                    options.ClientId = builder.Configuration["GOOGLE_CLIENT_ID"];
                    options.ClientSecret = builder.Configuration["GOOGLE_CLIENT_SECRET"];
                    options.SaveTokens = true;
                });

            // authorization policies
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireAdminRole", policy =>
                    policy.RequireRole("Admin"));

                options.AddPolicy("RequireCustomerRole", policy =>
                    policy.RequireRole("Customer"));
            });

            // CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder =>
                {
                    builder.WithOrigins(
                            "http://localhost:5173",     // React admin app
                            "https://localhost:7001",    // Razor Pages (HTTPS)
                            "http://localhost:5178")     // Razor Pages (HTTP)
                           .AllowAnyMethod()
                           .AllowAnyHeader()
                           .AllowCredentials();
                });
            });

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Seed admin data
            using (var scope = app.Services.CreateScope())
            {
                var seeder = scope.ServiceProvider.GetRequiredService<DbSeeder>();
                await seeder.SeedAsync();

                // Also seed OAuth clients
                await SeedOpenIddictDataAsync(scope.ServiceProvider);
            }

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseStaticFiles();
            app.UseHttpsRedirection();
            app.UseCors("CorsPolicy");

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }

        private static async Task SeedOpenIddictDataAsync(IServiceProvider serviceProvider)
        {
            var manager = serviceProvider.GetRequiredService<IOpenIddictApplicationManager>();

            // Customer application (Razor Pages)
            if (await manager.FindByClientIdAsync("customer-web-client") is null)
            {
                await manager.CreateAsync(new OpenIddictApplicationDescriptor
                {
                    ClientId = "customer-web-client",
                    ClientSecret = "customer-secret",
                    DisplayName = "Customer Web Portal",
                    RedirectUris = { new Uri("https://localhost:7251/signin-oidc") },
                    PostLogoutRedirectUris = { new Uri("https://localhost:7251/signout-callback-oidc") },
                    Permissions =
                    {
                        OpenIddictConstants.Permissions.Endpoints.Authorization,
                        OpenIddictConstants.Permissions.Endpoints.Token,
                        OpenIddictConstants.Permissions.Endpoints.EndSession,

                        OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,
                        OpenIddictConstants.Permissions.GrantTypes.Password,
                        OpenIddictConstants.Permissions.GrantTypes.RefreshToken,


                        OpenIddictConstants.Permissions.Scopes.Email,
                        OpenIddictConstants.Permissions.Scopes.Profile,
                        OpenIddictConstants.Permissions.Scopes.Roles,
                        OpenIddictConstants.Permissions.Prefixes.Scope + "api",
                        OpenIddictConstants.Scopes.OfflineAccess


                    }
                });
            }
            // Admin application
            if (await manager.FindByClientIdAsync("admin-web-client") is null)
            {
                await manager.CreateAsync(new OpenIddictApplicationDescriptor
                {
                    ClientId = "admin-web-client",
                    ClientSecret = "admin-secret",
                    DisplayName = "Admin Portal",
                    RedirectUris = { new Uri("http://localhost:5173/callback") },
                    PostLogoutRedirectUris = { new Uri("http://localhost:5173/logout-callback") },
                    Permissions =
                    {
                        OpenIddictConstants.Permissions.Endpoints.Authorization,
                        OpenIddictConstants.Permissions.Endpoints.Token,
                        OpenIddictConstants.Permissions.Endpoints.EndSession,

                        OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,
                        OpenIddictConstants.Permissions.GrantTypes.Password,
                        OpenIddictConstants.Permissions.GrantTypes.RefreshToken,
                        OpenIddictConstants.Permissions.GrantTypes.ClientCredentials,

                        OpenIddictConstants.Permissions.Scopes.Email,
                        OpenIddictConstants.Permissions.Scopes.Profile,
                        OpenIddictConstants.Permissions.Scopes.Roles,
                        OpenIddictConstants.Permissions.Prefixes.Scope + "api",
                        OpenIddictConstants.Scopes.OfflineAccess,
                    }
                });
            }
        }
    }
}