using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using SearchService.Entities;
using SearchService.Shared;

namespace SearchService.Feature;

public sealed class QueryProductsHandler
{
    private readonly IMongoCollection<MongoProduct> _collection;

    public QueryProductsHandler(IMongoClient client, IOptions<MongoOptions> options)
    {
        _collection = client.GetDatabase(options.Value.Database).GetCollection<MongoProduct>("products");
    }

    public async Task<QueryProductsResponse> Handle(QueryProducts query, CancellationToken ct)
    {
        var filter = Builders<MongoProduct>.Filter.Empty;

        if (!string.IsNullOrWhiteSpace(query.Query))
        {
            var pattern = new BsonRegularExpression(query.Query, "i");
            filter &= Builders<MongoProduct>.Filter.Or(
                Builders<MongoProduct>.Filter.Regex(p => p.Name, pattern),
                Builders<MongoProduct>.Filter.Regex(p => p.Description!, pattern)
            );
        }

        var docs = await _collection.Find(filter).ToListAsync(ct);

        var result = docs.Select(p => new SearchProductDTO
        {
            Id = p.Id,
            Name = p.Name,
            Price = p.Price,
            Description = p.Description,
            CategoryName = p.CategoryName
        }).ToList();

        return new QueryProductsResponse(result);
    }
}
