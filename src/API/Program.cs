using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OpenIddict.Abstractions;
using Application.Interfaces.Auth;
using Application.Interfaces.Carts;
using Application.Interfaces.Categories;
using Application.Interfaces.Products;
using Application.Interfaces.Reviews;
using Application.Services.Auth;
using Application.Services.Carts;
using Application.Services.Categories;
using Application.Services.Categories;
using Application.Services.Product;
using Application.Services.Review;
using Domain.Entities;
using DotNetEnv;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.OpenApi.Models;

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
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
            );
            builder.Services.AddAutoMapper(typeof(Program).Assembly, typeof(CategoryService).Assembly);
            builder.Services.AddRazorPages();

            // Register Identity
            builder.Services.AddIdentity<User, IdentityRole<Guid>>(options =>
            {
                options.SignIn.RequireConfirmedAccount = true;
            }).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();

            // Configure Identity 
            builder.Services.Configure<IdentityOptions>(options =>
            {
                options.ClaimsIdentity.UserNameClaimType = OpenIddictConstants.Claims.Name;
                options.ClaimsIdentity.UserIdClaimType = OpenIddictConstants.Claims.Subject;
                options.ClaimsIdentity.RoleClaimType = OpenIddictConstants.Claims.Role;
                options.ClaimsIdentity.EmailClaimType = OpenIddictConstants.Claims.Email;
            });
            
            //Different customer and admin scheme
            builder.Services.AddAuthentication()
                .AddCookie("AdminScheme",options =>
                {
                  options.Cookie.Name = ".AspNetCore.Identity.Admin";  
                  options.LoginPath = "/Account/Admin/AdminLogin";
                  options.ExpireTimeSpan = TimeSpan.FromDays(7);
                  options.SlidingExpiration = true;
                })
                .AddCookie("CustomerScheme",options =>
                {
                    options.Cookie.Name = ".AspNetCore.Identity.Customer";  
                    options.LoginPath = "/Account/Customer/CustomerLogin";
                    options.ExpireTimeSpan = TimeSpan.FromDays(7);
                    options.SlidingExpiration = true;
                });

            builder.Services.AddOpenIddict()
                .AddCore(options =>
                {
                    options.UseEntityFrameworkCore()
                           .UseDbContext<AppDbContext>();
                })
                .AddServer(options =>
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

            #region Register Services
            builder.Services.AddScoped<IProductRepository, ProductRepository>();
            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
            builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddScoped<ICartRepository, CartRepository>();
            builder.Services.AddScoped<ICartService, CartService>();
            builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
            builder.Services.AddScoped<IReviewService, ReviewService>();
            builder.Services.AddScoped<IEmailSender, EmailSender>();
            builder.Services.AddScoped<DbSeeder>();
            builder.Services.AddScoped<OpenIdSeeder>();
            #endregion
            
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
                    builder.WithOrigins("https://localhost:5173", "https://localhost:7001","http://localhost:5178")
                           .AllowAnyMethod()
                           .AllowAnyHeader()
                           .AllowCredentials();
                });
            });

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            // In your Program.cs, modify the AddSwaggerGen section
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "E-commerce API", Version = "v1" });
    
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer <Access Token>...\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });
    
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            var app = builder.Build();

            // Seed data
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

            app.UseStaticFiles();
            app.UseHttpsRedirection();
            app.UseCors("CorsPolicy");
            
            app.UseAuthentication();
            app.UseAuthorization();
            
            app.MapControllers();
            app.MapRazorPages();

            app.Run();
        }
    }
}