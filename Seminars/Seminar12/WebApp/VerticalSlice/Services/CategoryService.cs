using Microsoft.EntityFrameworkCore;
using VerticalSlice.Domain.Entities;
using VerticalSlice.DTOs;
using VerticalSlice.Infrastructure;

namespace VerticalSlice.Services;

public class CategoryService(WebAppDbContext db) : ICategoryService
{
    private readonly WebAppDbContext db = db;

    public async Task<IEnumerable<CategoryDTO>> GetCategories()
    {
        var categories = await db.Categories
            .AsNoTracking()
            .Select(c => new CategoryDTO
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description
            })
            .ToListAsync();

        return categories;
    }

    public async Task<CategoryDTO?> FindCategoryById(int id)
    {
        var category = await db.Categories
                        .AsNoTracking()
                        .FirstOrDefaultAsync(x => x.Id == id);

        if (category == null)
        {
            return null;
        }

        return new CategoryDTO
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description
        };
    }

    public async Task CreateCategory(CategoryDTO model)
    {
        if (model == null)
        {
            throw new ArgumentNullException(nameof(model));
        }

        var entity = new Category
        {
            Name = model.Name,
            Description = model.Description,
        };

        db.Categories.Add(entity);
        await db.SaveChangesAsync();

        model.Id = entity.Id;
    }

    public async Task UpdateCategory(CategoryDTO model)
    {
        if (model == null)
        {
            throw new ArgumentNullException(nameof(model));
        }

        var entity = await db.Categories.FirstOrDefaultAsync(x => x.Id == model.Id);

        if (entity == null)
        {
            throw new KeyNotFoundException("Category not found.");
        }

        entity.Name = model.Name;
        entity.Description = model.Description;

        await db.SaveChangesAsync();
    }

    public async Task DeleteCategory(int id)
    {
        if (id < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(id));
        }

        var entity = await db.Categories.FindAsync(id);

        if (entity == null)
        {
            return;
        }

        db.Categories.Remove(entity);
        await db.SaveChangesAsync();
    }
}
