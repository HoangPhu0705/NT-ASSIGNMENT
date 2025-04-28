using Application.Interfaces.Products;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
    }
}