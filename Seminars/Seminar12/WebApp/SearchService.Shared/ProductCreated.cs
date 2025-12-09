namespace SearchService.Shared;

public sealed record ProductCreated(
    int ProductId,
    string Name,
    string? Description,
    decimal Price,
    string? CategoryName);