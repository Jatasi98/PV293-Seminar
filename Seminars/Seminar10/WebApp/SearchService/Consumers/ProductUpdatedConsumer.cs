using Microsoft.Extensions.Options;
using MongoDB.Driver;
using SearchService.Entities;
using SearchService.Shared;

namespace SearchService.Consumers;

public sealed class ProductUpdatedConsumer
{
    private readonly IMongoCollection<MongoProduct> _collection;

    public ProductUpdatedConsumer(IMongoClient client, IOptions<MongoOptions> options)
    {
        _collection = client.GetDatabase(options.Value.Database).GetCollection<MongoProduct>("products");
    }

    public async Task Consume(ProductUpdated product, CancellationToken ct)
    {
        var update = Builders<MongoProduct>.Update
            .Set(p => p.Name, product.Name)
            .Set(p => p.Description, product.Description)
            .Set(p => p.Price, product.Price)
            .Set(p => p.CategoryName, product.CategoryName);

        await _collection.UpdateOneAsync(p => p.Id == product.ProductId, update, new UpdateOptions { IsUpsert = true }, ct);
    }
}
