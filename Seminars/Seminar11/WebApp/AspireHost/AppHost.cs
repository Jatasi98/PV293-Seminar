using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var rabbit = builder.AddConnectionString("rabbitmq");
var mongo = builder.AddConnectionString("mongodb");

builder.AddProject<VerticalSlice>("webapp")
    .WithReference(rabbit);

builder.AddProject<SearchService>("searchservice")
    .WithReference(rabbit)
    .WithReference(mongo)
    .WithEnvironment("Mongo__Database", "SearchDb");

builder.Build().Run();
