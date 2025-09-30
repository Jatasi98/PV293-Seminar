using DAL.Entities;

namespace BL.DTOs;

public record SearchResult(
    string Query,
    IReadOnlyList<Product> Products,
    IReadOnlyList<Category> Categories);
