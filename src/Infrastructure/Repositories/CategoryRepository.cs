using Application.Interfaces.Categories;
using Domain.Entities;
using Application.Interfaces.Categories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly AppDbContext _context;

    public CategoryRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Category>> GetAllAsync()
    {
        return await _context.Categories
            .Include(c => c.ParentCategory)
            .ToListAsync();
    }

    public async Task<Category> GetByIdAsync(Guid id)
    {
        return await _context.Categories
            .Include(c => c.ParentCategory)
            .Include(c => c.SubCategories)
            .FirstOrDefaultAsync(c => c.Id == id);
    }
    
    public async Task<IEnumerable<CategoryAttribute>> GetCategoryAttributesAsync(Guid categoryId)
    {
        return await _context.CategoryAttributes
            .Include(ca => ca.ProductVariantAttributes)
            .ThenInclude(pva => pva.Values)
            .Where(ca => ca.CategoryId == categoryId)
            .ToListAsync();
    }

    public async Task<Category> AddAsync(Category category)
    {
        await _context.Categories.AddAsync(category);
        await _context.SaveChangesAsync();
        return category;
    }

    public async Task UpdateAsync(Category category)
    {
        _context.Entry(category).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null)
            return false;

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Categories.AnyAsync(c => c.Id == id);
    }

    public async Task<bool> HasSubcategoriesAsync(Guid id)
    {
        return await _context.Categories.AnyAsync(c => c.ParentCategoryId == id);
    }

    public async Task<bool> HasProductsAsync(Guid id)
    {
        return await _context.Products.AnyAsync(p => p.CategoryId == id);
    }

    public async Task<IEnumerable<Category>> GetRootCategoriesAsync()
    {
        return await _context.Categories
            .Where(c => c.ParentCategoryId == null)
            .ToListAsync();
    }

    public async Task<IEnumerable<Category>> GetSubcategoriesAsync(Guid parentId)
    {
        return await _context.Categories
            .Where(c => c.ParentCategoryId == parentId)
            .ToListAsync();
    }
    
}