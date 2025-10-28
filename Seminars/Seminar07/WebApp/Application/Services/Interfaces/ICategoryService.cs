using Application.DTOs;

namespace Application.Services.Interfaces;

public interface ICategoryService
{
    Task<IEnumerable<CategoryDTO>> GetCategories();
    Task<CategoryDTO?> FindCategoryById(int id);
    Task CreateCategory(CategoryDTO model);
    Task UpdateCategory(CategoryDTO model);
    Task DeleteCategory(int id);
}
