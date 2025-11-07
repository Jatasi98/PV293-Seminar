using Microsoft.Extensions.Options;
using MongoDB.Driver;
using SearchService;
using SearchService.Entities;
using SearchService.Feature;
using Wolverine;
using Wolverine.RabbitMQ;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddOpenApi();

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

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

var client = app.Services.GetRequiredService<IMongoClient>();
var options = app.Services.GetRequiredService<IOptions<MongoOptions>>().Value;
var db = client.GetDatabase(options.Database);
var products = db.GetCollection<MongoProduct>("products");

await products.Indexes.CreateManyAsync(
[
    new CreateIndexModel<MongoProduct>(Builders<MongoProduct>.IndexKeys.Ascending(p => p.Name), new CreateIndexOptions { Name = "idx_name" }),
    new CreateIndexModel<MongoProduct>(Builders<MongoProduct>.IndexKeys.Ascending(p => p.CategoryName), new CreateIndexOptions { Name = "idx_categoryName" })
]);

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
