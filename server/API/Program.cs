using Application.Interfaces.Auth;
using Application.Services.Auth;
using Domain.Entities;
using DotNetEnv;
using Infrastructure.Data;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace API
{
    public class Program
    {
        public static async Task Main(string[] args) 
        {
            Env.Load();
            var builder = WebApplication.CreateBuilder(args);

            //Register DBContext
            builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            //Register Identity
            builder.Services.AddIdentity<User, IdentityRole<Guid>>()
                            .AddEntityFrameworkStores<AppDbContext>()
                            .AddDefaultTokenProviders();

            //Register Services
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IEmailSender, EmailSender>();
            builder.Services.AddScoped<ITokenService, TokenService>();
            builder.Services.AddScoped<DbSeeder>();


            //AUTHENTICATION CONFIGURATION
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["JWT_ISSUER"],
                    ValidAudience = builder.Configuration["JWT_AUDIENCE"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT_KEY"]))
                };   
            }).AddGoogle(options =>
            {
                options.ClientId = builder.Configuration["GOOGLE_CLIENT_ID"];
                options.ClientSecret = builder.Configuration["GOOGLE_CLIENT_SECRET"];
            });

            //CORS
            builder.Services.AddCors(options => 
            {
                options.AddPolicy("CorsPolicy", builder =>
                {
                    builder.WithOrigins("http://localhost:5173", "https://localhost:7289")
                           .AllowAnyMethod()
                           .AllowAnyHeader()
                           .AllowCredentials(); // Required for cookies
                });
            });


            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(
                options =>
                {
                    options.SwaggerDoc("v1", new OpenApiInfo
                    {
                        Title = "NT-ASSIGNMENT APIs",
                        Version = "v1"
                    });
                }
                );


            //BUILD THE APP
            var app = builder.Build();


            //SEED ADMIN DATA
            using (var scope = app.Services.CreateScope())
            {
                var seeder = scope.ServiceProvider.GetRequiredService<DbSeeder>();
                await seeder.SeedAsync();
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseStaticFiles(); 
            app.UseHttpsRedirection();
            app.UseCors("CorsPolicy");




            app.UseAuthentication();
            app.Use(async (context, next) =>
            {
                if (!context.Request.Headers.ContainsKey("Authorization"))
                {
                    var token = context.Request.Cookies["access_token"];
                    if (!string.IsNullOrEmpty(token))
                    {
                        context.Request.Headers["Authorization"] = $"Bearer {token}";
                    }
                }
                await next(context);
            });
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
