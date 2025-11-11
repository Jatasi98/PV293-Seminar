# Seminar 09 – From Modulith to Microservice, Aggregates, Product Search Service

In the previous seminar, we completed the transformation of our monolithic application into a modulith with an independent Orders module and introduced messaging through RabbitMQ and Wolverine.

In today’s seminar, we will continue this evolution by extracting a new microservice responsible for Product Search. This service will operate independently, synchronize data through integration events, and manage its own NoSQL database.

## Task 01: Product Aggregate Transformation

In this task, you will evolve the existing Product entity in the modulith so that it resembles a proper Aggregate Root. Keep the design lightweight but align it with aggregate rules (encapsulated state changes, invariant checks, domain events).

### Step 1: Aggregate sketch

Update the Product entity to encapsulate creation, updates, price changes, and soft delete.

```cs
public sealed class Product : BaseEntity
{
    public int Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public decimal Price { get; private set; }
    public int? CategoryId { get; private set; }
    public bool IsDeleted { get; private set; }

    public static Product Create(string name, string? description, decimal price, int? categoryId)
    {
        // TODO
    }

    public void UpdateDescription(string? description)
    {
        // TODO
    }

    public void ChangeCategory(int? categoryId)
    {
        // TODO
    }

    public void ChangePrice(??? newPrice)
    {
        // TODO
    }

    public void SoftDelete()
    {
        // TODO
    }
}
```

Define an Aggregate Root entity that handles its own state and validates business logic (e.g. the Price of a product must not be negative. If it is, prevent the operation and return information indicating that the update is invalid).

Transform Price into a Value Object that contains both the product’s Price and the currency in which the product is sold.

Update the DbContext configuration to ensure that this change does not break existing functionality.

### Step 2: Integration events (contracts)

Add or update contracts to represent creation, updates, and deletion. These are integration events that our future Search service will consume. Publish these via Wolverine in command handlers after persistence.

```cs
public sealed record ProductCreated(
    int ProductId,
    string Name,
    string? Description,
    decimal Price,
    string? CategoryName);


public sealed record ProductUpdated(
    int ProductId,
    string Name,
    string? Description,
    decimal Price,
    string? CategoryName);


public sealed record ProductDeleted(
    int ProductId);
```

### Step 03: Publish events via Wolverine

After saving changes in your command handlers, publish the corresponding integration event on the Wolverine bus.

```cs
public sealed class CreateProductHandler(AppDbContext db, IMessageBus bus) : IRequestHandler<CreateProductCommand, int>
{
    private readonly AppDbContext _db = db;
    private readonly IMessageBus _bus = bus;


    public async Task<int> Handle(CreateProductCommand request, CancellationToken ct)
    {
        var product = Product.Create(request.Name, request.Description, request.Price, request.CategoryId);
        _db.Products.Add(product);
        await _db.SaveChangesAsync(ct);

        // TODO

        await _bus.PublishAsync(/*TODO*/);

        return product.Id;
    }
}
```

## Task 02: Create the Search Service

Create a standalone Web API responsible for indexing and searching product data. It will receive integration events through RabbitMQ and maintain its own MongoDB database.

### Install packages:
```bash
dotnet add SearchService package Wolverine
dotnet add SearchService package Wolverine.RabbitMQ
dotnet add SearchService package MongoDB.Driver
```

### **Configuration** (`appsettings.json`)

```json
{
    "Mongo": {
        "ConnectionString": "mongodb://admin:admin@localhost:27017/?authSource=admin",
        "Database": "SearchDb"
    },
    "RabbitMq": {
    "ConnectionString": "amqp://guest:guest@localhost:5672"
    }
}
```

### **Mongo registration**

```cs
builder.Services.Configure<MongoOptions>(builder.Configuration.GetSection("Mongo"));
builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var opts = sp.GetRequiredService<IOptions<MongoOptions>>().Value;
    return new MongoClient(opts.ConnectionString);
});

public sealed class MongoOptions
{
    public string ConnectionString { get; set; } = default!;
    public string Database { get; set; } = "SearchDb";
}
```

### **Wolverine with RabbitMQ**

```csharp
builder.Host.UseWolverine(opts =>
{
    opts.UseRabbitMq(builder.Configuration["RabbitMq:ConnectionString"])
        .AutoProvision()
        .AutoPurgeOnStartup();

    // Discover message handlers in this service assembly
    opts.Discovery.IncludeAssembly(typeof(Program).Assembly);
});
```

## Task 03: Search Project: Model and Consumers

### Step 1: Create MongoDB document

Use a document schema that mirrors what the main app returns so the Search service can fully replace EF queries.

```csharp
public sealed class MongoProduct
{
    public string Name { get; set; } = default!;
    public decimal Price { get; set; }
    public string? Description { get; set; }
    public string? CategoryName { get; set; }
}
```

Just like the RabbitMQ Docker container, the NoSQL database also needs to be run in a container.

```bash
docker run -d --name mongo -p 27017:27017 -e MONGO_INITDB_ROOT_USERNAME=admin -e MONGO_INITDB_ROOT_PASSWORD=admin mongo:7
```

Create a collection on startup and basic indexes:

```csharp
var client = app.Services.GetRequiredService<IMongoClient>();
var options = app.Services.GetRequiredService<IOptions<MongoOptions>>().Value;
var db = client.GetDatabase(options.Database);
var products = db.GetCollection<MongoProduct>("products");

await products.Indexes.CreateManyAsync(new[]
{
    new CreateIndexModel<MongoProduct>(Builders<MongoProduct>.IndexKeys.Ascending(p => p.Name), new CreateIndexOptions { Name = "idx_name" }),
    new CreateIndexModel<MongoProduct>(Builders<MongoProduct>.IndexKeys.Ascending(p => p.CategoryId), new CreateIndexOptions { Name = "idx_categoryId" })
});
```

### Step 2: Consumers

Create each consumer in its own file to keep responsibilities clear and easy to modify. These cunsumer will handle our events created in Task 01.

#### ProductCreatedConsumer.cs
```cs
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
```

#### ProductUpdatedConsumer.cs
```cs
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
```

#### ProductDeletedConsumer.cs
```cs
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
```

## Task 04: Route Queries Through the Search Service

Replace EF query logic in the main app with a request/response call to the Search service over RabbitMQ using Wolverine. The Search service becomes the source for product lists.

### Step 1: DTO and query contract

```cs
public sealed class SearchProductDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public decimal Price { get; set; }
    public string? Description { get; set; }
    public string? CategoryName { get; set; }
}

public sealed record QueryProducts(string? Query);

public sealed record QueryProductsResponse(List<SearchProductDTO> Items);
```

### Step 2: Search service request handler

```cs
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
```

### Step 3: Main app calls Search via Wolverine request/response

In the main app where EF was used previously, replace it with Wolverine request/response over RabbitMQ:

```csharp
// Old EF code (for reference)
// var products = await db.Products
//     .Include(p => p.Category)
//     .AsNoTracking()
//     .Where(p => p.Name.Contains(query) || p.Description != null && p.Description.Contains(query))
//     .Select(product => new ProductDTO
//     {
//         Id = product.Id,
//         Name = product.Name,
//         Price = product.Price,
//         Description = product.Description,
//         IsDeleted = product.IsDeleted,
//         CategoryId = product.CategoryId,
//         CategoryName = product.Category != null ? product.Category.Name : null
//     })
//     .ToListAsync(ct);

// New request to Search service
public class SearchService(WebAppDbContext db, IMessageBus bus) : ISearchService
{
    //...
    private readonly IMessageBus _bus = bus;

    public async Task<SearchResult> SearchAsync(string? query, int? maxPerType = null, CancellationToken ct = default)
    {
        //...
        var result = await _bus.InvokeAsync<QueryProducts, List<ProductDTO>>(new QueryProducts(query), ct);

        //...

        return new SearchResult();
    }
}
```

Update Program.cs accordingly so that the application and the search service can run together.

**MVC - Program.cs**
```cs
builder.Host.UseWolverine(opts =>
{
    opts.UseRabbitMq(new Uri("amqp://guest:guest@localhost:5672"))
        .AutoProvision()
        .AutoPurgeOnStartup();

    opts.PublishAllMessages()
        .ToRabbitQueue("search_service");

    opts.ListenToRabbitQueue("webapp_replies");

    opts.Discovery.IncludeAssembly(typeof(QueryProducts).Assembly);

    opts.Discovery.IncludeAssembly(typeof(Program).Assembly);
});
```
**SearchService - Program.cs**
```cs
builder.Services.Configure<MongoOptions>(builder.Configuration.GetSection("Mongo"));
builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var opts = sp.GetRequiredService<IOptions<MongoOptions>>().Value;
    return new MongoClient(opts.ConnectionString);
});

builder.Host.UseWolverine(opts =>
{
    opts.UseRabbitMq(builder.Configuration["RabbitMq:ConnectionString"] ?? throw new ArgumentNullException("Connection string for RabbitMQ was not found"))
        .AutoProvision()
        .AutoPurgeOnStartup();

    opts.ListenToRabbitQueue("search_service");

    opts.Discovery.IncludeAssembly(typeof(QueryProductsHandler).Assembly);

    opts.Discovery.IncludeAssembly(typeof(Program).Assembly);
});

//...

var client = app.Services.GetRequiredService<IMongoClient>();
var options = app.Services.GetRequiredService<IOptions<MongoOptions>>().Value;
var db = client.GetDatabase(options.Database);
var products = db.GetCollection<MongoProduct>("products");

await products.Indexes.CreateManyAsync(
[
    new CreateIndexModel<MongoProduct>(Builders<MongoProduct>.IndexKeys.Ascending(p => p.Name), new CreateIndexOptions { Name = "idx_name" }),
    new CreateIndexModel<MongoProduct>(Builders<MongoProduct>.IndexKeys.Ascending(p => p.CategoryName), new CreateIndexOptions { Name = "idx_categoryName" })
]);
```

This turns the main app’s search endpoint into a thin proxy that delegates to the Search service over RabbitMQ.

## Bonus: Backfill Existing Products into MongoDB

We need existing products in MongoDB so that search results are complete on day one. The following is a simple sketch that pulls pages from the main app and bulk upserts into MongoDB.

Steps:
#### 1: Define paging contracts
#### 2: Backfill runner in the Search service