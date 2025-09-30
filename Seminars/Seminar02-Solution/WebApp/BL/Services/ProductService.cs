using DAL;
using DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace BL.Services;

public class ProductService : IProductService
{
    private readonly WebAppDbContext _db;

    public ProductService(WebAppDbContext db)
    {
        _db = db;
    }

    public async Task<Product?> GetProduct(int productId)
    {
        var product = await _db.Products
            .Include(p => p.Category)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == productId);

        return product;
    }
}
