// Infrastructure/Data/DbSeeder.cs
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public class DbSeeder
{
    private readonly AppDbContext _context;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;

    public DbSeeder(AppDbContext context, UserManager<User> userManager, RoleManager<IdentityRole<Guid>> roleManager)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task SeedAsync()
    {
        await _context.Database.MigrateAsync();

        string[] roles = { "User", "Admin" };
        foreach (var role in roles)
        {
            if (!await _roleManager.RoleExistsAsync(role))
            {
                await _roleManager.CreateAsync(new IdentityRole<Guid> { Name = role, NormalizedName = role.ToUpper() });
            }
        }

        // Seed Admin
        var adminEmail = "admin@nextech.com";
        var adminUser = await _userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            adminUser = new User
            {
                FirstName = "Ad",
                LastName = "min",
                UserName = "admin",
                Email = adminEmail,
                CreatedAt = DateTime.UtcNow,
                EmailConfirmed = true
            };
            var result = await _userManager.CreateAsync(adminUser, "Abc123!");
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(adminUser, "Admin");
            }
            else
            {
                throw new Exception($"Failed to seed admin: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
        }
    }
}