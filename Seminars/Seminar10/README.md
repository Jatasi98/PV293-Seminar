# Seminar 10 – Deploying Microservices with .NET Aspire

## Finishing from the Previous Seminar

In the previous seminar, we created a SearchService microservice that handled product data updates through RabbitMQ and persisted them into MongoDB. Our main application remained the primary source of truth and published product events to the message bus.

Today, we will connect all components under a unified orchestration environment using .NET Aspire.

## Task 01: Introducing .NET Aspire

The .NET Aspire framework simplifies the process of managing multiple applications and related dependencies (such as databases, queues, and APIs). It allows developers to run complex distributed systems locally with minimal configuration.

### Step 01: Install Aspire Workload

Make sure you have the Aspire workload installed.

You can install it via the .NET CLI:

```bash
dotnet workload install aspire
```

Alternatively, update your SDK if Aspire is not available in your version.

### Step 02: Create a New Aspire Host Project

Create a new project in your existing solution that will serve as the orchestrator for all services. This project will act as the entry point for launching all microservices and dependencies.

### Step 03: Add Existing Projects to Aspire

Reference these projects from your new Aspire Orchestrator project.

## Task 02: Configuring Aspire Components

We will now register both applications and their dependencies inside Aspire.

### Open AppHost.cs in the Orchestrator

Inside your AspireHost project, open AppHost.cs and configure the services as shown below.

```cs
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
```

Update the appsettings file in your AspireHost project to include the necessary configurations, ensuring the application uses the existing connections for RabbitMQ and MongoDB.

```bash
  "ConnectionStrings": {
    "rabbitmq": "amqp://guest:guest@localhost:5672",
    "mongodb": "mongodb://admin:admin@localhost:27017/?authSource=admin"
  }
```

This configuration defines the complete system:
* RabbitMQ container for messaging.
* MongoDB container for search data.
* MainApp connected to RabbitMQ.
* SearchService connected to both MongoDB and RabbitMQ.

Update the Program.cs files in both the WebApp and SearchService projects to incorporate the necessary modifications, enabling integration with the features provided by Aspire.

**WebApp Program.cs**

```cs
builder.Host.UseWolverine(opts =>
{
    var rabbitConnection = builder.Configuration.GetConnectionString("rabbitmq");

    if (string.IsNullOrEmpty(rabbitConnection))
    {
        throw new ArgumentException("RabbitMQ is not configured");
    }

    opts.UseRabbitMq(new Uri(rabbitConnection))
        .AutoProvision()
        .AutoPurgeOnStartup();

    opts.PublishAllMessages()
        .ToRabbitQueue("search_service");

    opts.ListenToRabbitQueue("webapp_replies");

    opts.Discovery.IncludeAssembly(typeof(QueryProducts).Assembly);

    opts.Discovery.IncludeAssembly(typeof(Program).Assembly);
});
```

**SearchService Program.cs**

```cs
builder.Services.Configure<MongoOptions>(x =>
{
    x.ConnectionString = builder.Configuration.GetConnectionString("mongodb")
        ?? throw new ArgumentNullException("MongoDb connection not configured");

    x.Database = builder.Configuration["Mongo:Database"]
        ?? throw new ArgumentNullException("MongoDb database not configured");
});

builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var opts = sp.GetRequiredService<IOptions<MongoOptions>>().Value;
    return new MongoClient(opts.ConnectionString);
});

builder.Host.UseWolverine(opts =>
{
    var rabbitConnection = builder.Configuration.GetConnectionString("rabbitmq");

    if (string.IsNullOrEmpty(rabbitConnection))
    {
        throw new ArgumentException("RabbitMQ is not configured");
    }

    opts.UseRabbitMq(new Uri(rabbitConnection))
        .AutoProvision()
        .AutoPurgeOnStartup();

    opts.ListenToRabbitQueue("search_service");

    opts.Discovery.IncludeAssembly(typeof(QueryProductsHandler).Assembly);

    opts.Discovery.IncludeAssembly(typeof(Program).Assembly);
});
```

## Task 03: Running and Testing the Full Solution

Now that Aspire knows about all services and dependencies, we can run the system as a single distributed application. Run the Aspire Orchestrator project. After startup, Aspire provides a local dashboard that shows all running services, dependencies, and their status.

You should see Aspire automatically start:
* RabbitMQ container
* MongoDB container
* MainApp
* SearchService

### Verify Communication Flow
1. Open the MainApp
2. Create a new Product from your existing product form or API endpoint.
3. Query for that product using the SearchService
4. Confirm that the product data appears in MongoDB, showing successful propagation through RabbitMQ.
