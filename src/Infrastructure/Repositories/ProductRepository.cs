using Application.Interfaces.Products;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SharedViewModels.Product;

namespace Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Images)
                .Include(p => p.Variants)
                .Include(p => p.Variants).ThenInclude(v => v.AttributeValues)
                .ThenInclude(av => av.ProductVariantAttribute)
                .ThenInclude(a => a.CategoryAttribute)
                .ToListAsync();
        }

        public async Task<Product> GetByIdAsync(Guid id)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Images)
                .Include(p => p.Variants)
                .Include(p => p.Variants).ThenInclude(v => v.AttributeValues)
                .ThenInclude(av => av.ProductVariantAttribute)
                .ThenInclude(a => a.CategoryAttribute)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Product> AddAsync(Product product)
        {
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task UpdateAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var product = await _context.Products
                .Include(p => p.Images)
                .Include(p => p.Variants)
                .ThenInclude(v => v.AttributeValues)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
                return false;

            // Remove images first
            if (product.Images != null && product.Images.Any())
            {
                _context.ProductImages.RemoveRange(product.Images);
            }

            // Remove variants and their attributes
            if (product.Variants != null && product.Variants.Any())
            {
                foreach (var variant in product.Variants)
                {
                    if (variant.AttributeValues != null && variant.AttributeValues.Any())
                    {
                        _context.Set<VariantAttributeValue>().RemoveRange(variant.AttributeValues);
                    }
                }
                _context.ProductVariants.RemoveRange(product.Variants);
            }


            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Products.AnyAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Product>> GetByCategoryAsync(Guid categoryId)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Images)
                .Include(p => p.Variants)
                .Include(p => p.Variants).ThenInclude(v => v.AttributeValues)
                .ThenInclude(av => av.ProductVariantAttribute)
                .ThenInclude(a => a.CategoryAttribute)
                .Where(p => p.CategoryId == categoryId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Images)
                .Include(p => p.Variants)
                .Where(p => p.Name.Contains(searchTerm) || 
                           (p.Description != null && p.Description.Contains(searchTerm)))
                .ToListAsync();
        }

        public async Task DeleteProductImagesAsync(IEnumerable<Guid> imageIds)
        {
            var imagesToDelete = await _context.ProductImages
                .Where(img => imageIds.Contains(img.Id))
                .ToListAsync();
        
            if (imagesToDelete.Any())
            {
                _context.ProductImages.RemoveRange(imagesToDelete);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteProductVariantsAsync(IEnumerable<Guid> variantIds)
        {
            var variantsToDelete = await _context.ProductVariants
                .Include(v => v.AttributeValues)
                .Where(v => variantIds.Contains(v.Id))
                .ToListAsync();
        
            if (variantsToDelete.Any())
            {
                foreach (var variant in variantsToDelete)
                {
                    if (variant.AttributeValues != null && variant.AttributeValues.Any())
                    {
                        _context.Set<VariantAttributeValue>().RemoveRange(variant.AttributeValues);
                    }
                }
                _context.ProductVariants.RemoveRange(variantsToDelete);
                await _context.SaveChangesAsync();
            }
        }

       public async Task UpdateVariantAttributesAsync(ProductVariant variant, IEnumerable<UpdateVariantAttributeRequest> attributes)
        {
            // 1. Handle attribute removal
            if (variant.AttributeValues != null)
            {
                var attributesToRemove = variant.AttributeValues
                    .Where(av => !attributes.Any(a => a.Name == av.Name && !a.IsDeleted))
                    .ToList();

                if (attributesToRemove.Any())
                {
                    _context.Set<VariantAttributeValue>().RemoveRange(attributesToRemove);
                }
            }
            else
            {
                variant.AttributeValues = new List<VariantAttributeValue>();
            }

            // 2. Update and add attributes
            foreach (var attr in attributes.Where(a => !a.IsDeleted))
            {
                var existingAttr = variant.AttributeValues?
                    .FirstOrDefault(a => a.Name == attr.Name);

                if (existingAttr != null)
                {
                    existingAttr.Value = attr.Value;
                }
                else
                {
                    // Find or create appropriate ProductVariantAttribute
                    var productVariantAttribute = await _context.Set<ProductVariantAttribute>()
                        .FirstOrDefaultAsync(pva => 
                            pva.ProductId == variant.ProductId && 
                            pva.CategoryAttribute.Name == attr.Name);

                    if (productVariantAttribute == null)
                    {
                        var categoryAttribute = await _context.Set<CategoryAttribute>()
                            .FirstOrDefaultAsync(ca => ca.Name == attr.Name);

                        if (categoryAttribute == null)
                        {
                            // Create new CategoryAttribute if it doesn't exist
                            categoryAttribute = new CategoryAttribute
                            {
                                Id = Guid.NewGuid(),
                                Name = attr.Name,
                                IsFilterable = true
                            };
                            _context.Set<CategoryAttribute>().Add(categoryAttribute);
                            await _context.SaveChangesAsync();
                        }

                        // Create new ProductVariantAttribute
                        productVariantAttribute = new ProductVariantAttribute
                        {
                            Id = Guid.NewGuid(),
                            ProductId = variant.ProductId,
                            CategoryAttributeId = categoryAttribute.Id
                        };
                        _context.Set<ProductVariantAttribute>().Add(productVariantAttribute);
                        await _context.SaveChangesAsync();
                    }

                    // Add new attribute value
                    var attrValue = new VariantAttributeValue
                    {
                        ProductVariantId = variant.Id,
                        ProductVariantAttributeId = productVariantAttribute.Id,
                        Name = attr.Name,
                        Value = attr.Value
                    };
                    variant.AttributeValues.Add(attrValue);
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}