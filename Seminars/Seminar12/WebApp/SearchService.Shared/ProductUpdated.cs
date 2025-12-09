namespace SearchService.Shared;

public sealed record ProductUpdated(
    int ProductId,
    string Name,
    string? Description,
    decimal Price,
    string? CategoryName);
