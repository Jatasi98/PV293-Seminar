using BL.DTOs;

namespace BL.Services;

public interface IProductService
{
    Task<ProductDTO?> GetProduct(int productId);
    Task<List<ProductDTO>> GetProducts();
    Task<ProductDTO?> FindProductById(int id);
    Task CreateProduct(ProductDTO product);
    Task UpdateProduct(ProductDTO product);
}
