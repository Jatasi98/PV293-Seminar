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

builder.AddContainer(name: "prometheus", image: "prom/prometheus:v2.54.1")
    .WithBindMount("./prometheus.yml", "/etc/prometheus/prometheus.yml")
    .WithEndpoint(targetPort: 9090, port: 9090, scheme: "http");

builder.AddContainer("grafana", "grafana/grafana:11.2.0")
    .WithEndpoint(targetPort: 3000, port: 3000, scheme: "http")
    .WithEnvironment("GF_SECURITY_ADMIN_USER", "admin")
    .WithEnvironment("GF_SECURITY_ADMIN_PASSWORD", "admin")
    .WithEnvironment("GF_USERS_ALLOW_SIGN_UP", "false");

builder.Build().Run();
