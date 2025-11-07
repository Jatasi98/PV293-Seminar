using Microsoft.Extensions.Options;
using MongoDB.Driver;
using SearchService.Entities;
using SearchService.Shared;

namespace SearchService.Consumers;

public sealed class ProductDeletedConsumer
{
    private readonly IMongoCollection<MongoProduct> _collection;

    public ProductDeletedConsumer(IMongoClient client, IOptions<MongoOptions> options)
    {
        _collection = client.GetDatabase(options.Value.Database).GetCollection<MongoProduct>("products");
    }

    public async Task Consume(ProductDeleted e, CancellationToken ct)
    {
        await _collection.DeleteOneAsync(p => p.Id == e.ProductId, ct);
    }
}
