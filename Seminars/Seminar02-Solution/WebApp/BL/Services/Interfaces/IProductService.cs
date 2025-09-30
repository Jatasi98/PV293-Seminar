using DAL.Entities;

namespace BL.Services;

public interface IProductService
{
    Task<Product?> GetProduct(int productId);
}
