using Microsoft.Extensions.Options;
using MongoDB.Driver;
using SearchService;
using SearchService.Entities;
using SearchService.Shared;

namespace SearchServicproduct.Consumers;

public sealed class ProductCreatedConsumer
{
    private readonly IMongoCollection<MongoProduct> _collection;

    public ProductCreatedConsumer(IMongoClient client, IOptions<MongoOptions> options)
    {
        _collection = client.GetDatabase(options.Value.Database).GetCollection<MongoProduct>("products");
    }

    public async Task Consume(ProductCreated product, CancellationToken ct)
    {
        var doc = new MongoProduct
        {
            Id = product.ProductId,
            Name = product.Name,
            Price = product.Price,
            Description = product.Description,
            CategoryName = product.CategoryName
        };

        await _collection.InsertOneAsync(doc);
    }
}
