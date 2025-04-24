using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OpenIddict.Abstractions;
using Application.Interfaces.Auth;
using Application.Interfaces.Categories;
using Application.Services.Auth;
using Application.Services.Categories;
using Domain.Entities;
using DotNetEnv;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity.UI.Services;

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
            builder.Services.AddAutoMapper(typeof(Program).Assembly, typeof(CategoryService).Assembly);
            // Add Razor Pages support
            builder.Services.AddRazorPages();

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
                options.LoginPath = "/Account/Login"; 
                options.Cookie.Name = "nextech_auth";
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.SameSite = SameSiteMode.Strict;
                options.ExpireTimeSpan = TimeSpan.FromHours(1);
                options.SlidingExpiration = true;
            });

            builder.Services.AddOpenIddict().AddCore(options =>
                {
                    options.UseEntityFrameworkCore()
                           .UseDbContext<AppDbContext>();
                }).AddServer(options =>
                {
                    options.AllowAuthorizationCodeFlow()
                           .AllowRefreshTokenFlow();
                    options.SetTokenEndpointUris("connect/token")
                           .SetAuthorizationEndpointUris("connect/authorize")
                           .SetUserInfoEndpointUris("connect/userinfo")
                           .SetEndSessionEndpointUris("connect/logout");
                    
                    options.AddDevelopmentEncryptionCertificate()
                           .AddDevelopmentSigningCertificate();
                    options.RegisterScopes("openid", "email", "profile", "roles", "offline_access", "api");
                    
                    options.AcceptAnonymousClients();
                    options.DisableAccessTokenEncryption();
                    
                    options.UseAspNetCore()
                           .EnableTokenEndpointPassthrough()
                           .EnableAuthorizationEndpointPassthrough()
                           .EnableUserInfoEndpointPassthrough()
                           .EnableEndSessionEndpointPassthrough();
                    
                    options.SetAccessTokenLifetime(TimeSpan.FromHours(1));
                    options.SetRefreshTokenLifetime(TimeSpan.FromDays(14));
                })
                .AddValidation(options =>
                {
                    options.UseLocalServer();
                    options.UseAspNetCore();
                });

            // Register Services
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
            builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddScoped<IEmailSender, EmailSender>();
            builder.Services.AddScoped<DbSeeder>();
            builder.Services.AddScoped<OpenIdSeeder>();

            // Configure OAuth providers
            builder.Services.AddAuthentication()
                .AddGoogle(options =>
                {
                    options.ClientId = builder.Configuration["GOOGLE_CLIENT_ID"];
                    options.ClientSecret = builder.Configuration["GOOGLE_CLIENT_SECRET"];
                    options.SaveTokens = true;
                });

            // Authorization policies
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireAdminRole", policy =>
                    policy.RequireRole("Admin"));
                options.AddPolicy("RequireCustomerRole", policy =>
                    policy.RequireRole("Customer"));
            });

            // CORS configuration
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder =>
                {
                    builder.WithOrigins(
                            "http://localhost:5173",
                            "https://localhost:7001",
                            "http://localhost:5178")
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
                var openIdSeeder = scope.ServiceProvider.GetRequiredService<OpenIdSeeder>();
                await seeder.SeedAsync();
                await openIdSeeder.SeedOpenIddictDataAsync(scope.ServiceProvider);
            }

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseStaticFiles(); // Enable static files for CSS/JS
            app.UseHttpsRedirection();
            app.UseCors("CorsPolicy");
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            app.MapRazorPages(); // Add Razor Pages

            app.Run();
        }
    }
}