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
    public DbSet<ProductVariant> ProductVariants { get; set; }
    public DbSet<ProductImage> ProductImages { get; set; }
    public DbSet<CategoryAttribute> CategoryAttributes { get; set; }
    public DbSet<ProductVariantAttribute> ProductVariantAttributes { get; set; }
    public DbSet<VariantAttributeValue> VariantAttributeValues { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Cart> Cart { get; set; }
    public DbSet<Discount> Discounts { get; set; }
    public DbSet<ProductReview> ProductReviews { get; set; }
    public DbSet<Wishlist> Wishlist { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<ShippingMethod> ShippingMethods { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        builder.UseOpenIddict();
        
        builder.Entity<User>().ToTable("Users");
        
        // Category hierarchy
        builder.Entity<Category>()
            .HasOne(c => c.ParentCategory)
            .WithMany(c => c.SubCategories)
            .HasForeignKey(c => c.ParentCategoryId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

        // Product relationships
        builder.Entity<Product>()
            .HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.Entity<ProductVariant>()
            .HasOne(pv => pv.Product)
            .WithMany(p => p.Variants)
            .HasForeignKey(pv => pv.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<ProductVariant>()
            .HasIndex(pv => pv.SKU)
            .IsUnique();
            
        // ProductImage relationships
        builder.Entity<ProductImage>()
            .HasOne(pi => pi.Product)
            .WithMany(p => p.Images)
            .HasForeignKey(pi => pi.ProductId)
            .OnDelete(DeleteBehavior.NoAction); 
        
        builder.Entity<ProductImage>()
            .HasOne(pi => pi.ProductVariant)
            .WithMany(pv => pv.Images)
            .HasForeignKey(pi => pi.ProductVariantId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);

        // Category Attribute relationships
        builder.Entity<CategoryAttribute>()
            .HasOne(ca => ca.Category)
            .WithMany()
            .HasForeignKey(ca => ca.CategoryId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<ProductVariantAttribute>()
            .HasOne(pva => pva.Product)
            .WithMany(p => p.VariantAttributes)
            .HasForeignKey(pva => pva.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.Entity<ProductVariantAttribute>()
            .HasOne(pva => pva.CategoryAttribute)
            .WithMany(ca => ca.ProductVariantAttributes)
            .HasForeignKey(pva => pva.CategoryAttributeId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.Entity<VariantAttributeValue>()
            .HasOne(vav => vav.ProductVariantAttribute)
            .WithMany(pva => pva.Values)
            .HasForeignKey(vav => vav.ProductVariantAttributeId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.Entity<VariantAttributeValue>()
            .HasOne(vav => vav.ProductVariant)
            .WithMany(pv => pv.AttributeValues)
            .HasForeignKey(vav => vav.ProductVariantId)
            .OnDelete(DeleteBehavior.Restrict);

        // Order relationships
        builder.Entity<OrderItem>()
            .HasOne(oi => oi.Order)
            .WithMany(o => o.OrderItems)
            .HasForeignKey(oi => oi.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.Entity<OrderItem>()
            .HasOne(oi => oi.Product)
            .WithMany()
            .HasForeignKey(oi => oi.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.Entity<OrderItem>()
            .HasOne(oi => oi.ProductVariant)
            .WithMany(pv => pv.OrderItems)
            .HasForeignKey(oi => oi.ProductVariantId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.Entity<OrderItem>()
            .HasOne(oi => oi.Discount)
            .WithMany(d => d.OrderItems)
            .HasForeignKey(oi => oi.DiscountId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);

        // Payment relationship
        builder.Entity<Payment>()
            .HasOne(p => p.Order)
            .WithMany(o => o.Payments)
            .HasForeignKey(p => p.OrderId)
            .OnDelete(DeleteBehavior.NoAction);

        // Cart relationships
        builder.Entity<Cart>()
            .HasOne(c => c.User)
            .WithMany()
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.Entity<Cart>()
            .HasOne(c => c.Product)
            .WithMany()
            .HasForeignKey(c => c.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.Entity<Cart>()
            .HasOne(c => c.ProductVariant)
            .WithMany(pv => pv.CartItems)
            .HasForeignKey(c => c.ProductVariantId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.Entity<Cart>()
            .HasOne(c => c.Discount)
            .WithMany(d => d.CartItems)
            .HasForeignKey(c => c.DiscountId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);

        // Wishlist relationships
        builder.Entity<Wishlist>()
            .HasOne(w => w.User)
            .WithMany()
            .HasForeignKey(w => w.UserId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.Entity<Wishlist>()
            .HasOne(w => w.Product)
            .WithMany()
            .HasForeignKey(w => w.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.Entity<Wishlist>()
            .HasOne(w => w.ProductVariant)
            .WithMany(pv => pv.WishlistItems)
            .HasForeignKey(w => w.ProductVariantId)
            .OnDelete(DeleteBehavior.Restrict);

        // Product review relationships
        builder.Entity<ProductReview>()
            .HasOne(pr => pr.Product)
            .WithMany(p => p.Reviews)
            .HasForeignKey(pr => pr.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
            
        // Important indexes
        builder.Entity<Discount>()
            .HasIndex(d => d.Code)
            .IsUnique();
            
        builder.Entity<Product>()
            .HasIndex(p => p.Name);
            
        builder.Entity<Category>()
            .HasIndex(c => c.Name);
            
        builder.Entity<Order>()
            .HasIndex(o => o.OrderDate);

        // Decimal precision 
        builder.Entity<ShippingMethod>()
            .Property(sm => sm.Cost)
            .HasPrecision(18, 2);

        builder.Entity<ProductVariant>()
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