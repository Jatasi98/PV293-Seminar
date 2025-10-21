using BL.DTOs;
using DAL;
using DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace BL.Services;

public class CategoryService : ICategoryService
{
    private readonly WebAppDbContext _db;

    public CategoryService(WebAppDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<CategoryDTO>> GetCategories()
    {
        var categories = await _db.Categories
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
        var category = await _db.Categories
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

        _db.Categories.Add(entity);
        await _db.SaveChangesAsync();

        model.Id = entity.Id;
    }

    public async Task UpdateCategory(CategoryDTO model)
    {
        if (model == null)
        {
            throw new ArgumentNullException(nameof(model));
        }

        var entity = await _db.Categories.FirstOrDefaultAsync(x => x.Id == model.Id);

        if (entity == null)
        {
            throw new KeyNotFoundException("Category not found.");
        }

        entity.Name = model.Name;
        entity.Description = model.Description;

        await _db.SaveChangesAsync();
    }

    public async Task DeleteCategory(int id)
    {
        if (id < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(id));
        }

        var entity = await _db.Categories.FindAsync(id);

        if (entity == null)
        {
            return;
        }

        _db.Categories.Remove(entity);
        await _db.SaveChangesAsync();
    }
}
