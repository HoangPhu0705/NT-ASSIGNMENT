// Infrastructure/Data/AppDbContext.cs
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Data;

public class AppDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<ProductImage> ProductImages { get; set; }
    public DbSet<CustomOption> CustomOptions { get; set; }
    public DbSet<ProductCustomOption> ProductCustomOptions { get; set; }
    public DbSet<CustomValue> CustomValues { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Cart> Cart { get; set; }
    public DbSet<Discount> Discounts { get; set; }
    public DbSet<DiscountProduct> DiscountProducts { get; set; }
    public DbSet<ProductReview> ProductReviews { get; set; }
    public DbSet<Wishlist> Wishlist { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<ShippingMethod> ShippingMethods { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        builder.UseOpenIddict();
        
        builder.Entity<User>().ToTable("Users");
        
        
        // Cascade 
        builder.Entity<Category>()
            .HasOne(c => c.ParentCategory)
            .WithMany(c => c.SubCategories)
            .HasForeignKey(c => c.ParentCategoryId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<ProductImage>()
            .HasOne(pi => pi.Product)
            .WithMany(p => p.Images)
            .HasForeignKey(pi => pi.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<ProductCustomOption>()
            .HasOne(pco => pco.Product)
            .WithMany(p => p.ProductCustomOptions)
            .HasForeignKey(pco => pco.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<ProductCustomOption>()
            .HasOne(pco => pco.CustomOption)
            .WithMany(co => co.ProductCustomOptions)
            .HasForeignKey(pco => pco.CustomOptionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<CustomValue>()
            .HasOne(cv => cv.ProductCustomOption)
            .WithMany(pco => pco.CustomValues)
            .HasForeignKey(cv => cv.ProductCustomOptionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<OrderItem>()
            .HasOne(oi => oi.Order)
            .WithMany(o => o.OrderItems)
            .HasForeignKey(oi => oi.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        // Fix: Change Payments -> Order to NO ACTION
        builder.Entity<Payment>()
            .HasOne(p => p.Order)
            .WithMany(o => o.Payments)
            .HasForeignKey(p => p.OrderId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Entity<Cart>()
            .HasOne(c => c.User)
            .WithMany()
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Wishlist>()
            .HasOne(w => w.User)
            .WithMany()
            .HasForeignKey(w => w.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Discount>()
            .HasIndex(d => d.Code)
            .IsUnique();

        // Decimal precision 
        builder.Entity<ShippingMethod>()
            .Property(sm => sm.Cost)
            .HasPrecision(18, 2);

        builder.Entity<Product>()
            .Property(p => p.Price)
            .HasPrecision(18, 2);

        builder.Entity<Order>()
            .Property(o => o.TotalAmount)
            .HasPrecision(18, 2);

        builder.Entity<OrderItem>()
            .Property(oi => oi.UnitPrice)
            .HasPrecision(18, 2);
        builder.Entity<OrderItem>()
            .Property(oi => oi.DiscountedPrice)
            .HasPrecision(18, 2);

        builder.Entity<Discount>()
            .Property(d => d.DiscountValue)
            .HasPrecision(18, 2);

        builder.Entity<Discount>()
            .Property(d => d.MinOrderAmount)
            .HasPrecision(18, 2);

        builder.Entity<Payment>()
            .Property(p => p.Amount)
            .HasPrecision(18, 2);
    }
}