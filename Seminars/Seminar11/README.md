# Seminar 11 – Observability and Telemetry

In this seminar, we will extend our existing Aspire-based setup by introducing **observability and telemetry** to our distributed application.
We will configure **Grafana** for monitoring metrics, integrate **OpenTelemetry** for distributed tracing, and implement centralized **structured logging** using **Serilog** and **Elasticsearch**.
This configuration will provide complete visibility into system health, performance, and request flow across all running services.

---

## Task 01: Grafana Integration with Aspire

Grafana is a monitoring and visualization tool that enables us to track application metrics in real time.
Using Aspire, we can automatically provision and configure Grafana alongside our existing services.
Grafana connects to Aspire’s built-in Prometheus endpoint, which collects metrics from all running applications.

### Step 01: Add Grafana to the Aspire Orchestrator

Open the Orchestrator project and modify the `Program.cs` file to include a Grafana container.
Grafana will automatically connect to the metrics exposed by Aspire.

```cs
// Add Grafana
var grafana = builder.AddContainer("grafana", "grafana/grafana:10.4.1")
    .WithPort(3000)
    .WithEnvironment("GF_SECURITY_ADMIN_USER", "admin")
    .WithEnvironment("GF_SECURITY_ADMIN_PASSWORD", "admin")
    .WithEnvironment("GF_USERS_ALLOW_SIGN_UP", "false");
```

Once started, access Grafana at [http://localhost:3000](http://localhost:3000) with credentials:

```
Username: admin
Password: admin
```

Aspire exposes a Prometheus-compatible endpoint by default, so Grafana automatically retrieves metrics from all connected projects.
This allows you to view live dashboards showing request counts, latency, and memory usage across your applications.

---

## Task 02: OpenTelemetry Integration

**OpenTelemetry enables distributed tracing and metrics collection across services.**
By connecting the Main Application and SearchService to the Aspire telemetry pipeline, we can visualize the full request flow through RabbitMQ, APIs, and databases.

### Step 01: Install Required Packages

Install OpenTelemetry packages in both the **MainApp** and **SearchService** projects:

```bash
dotnet add package OpenTelemetry
dotnet add package OpenTelemetry.Extensions.Hosting
dotnet add package OpenTelemetry.Exporter.OpenTelemetryProtocol
dotnet add package OpenTelemetry.Instrumentation.AspNetCore
dotnet add package OpenTelemetry.Instrumentation.Http
dotnet add package OpenTelemetry.Instrumentation.SqlClient
```

### Step 02: Configure OpenTelemetry in Program.cs

In each project, register tracing and metrics before building the application:

```cs
builder.Services.AddOpenTelemetry()
    .WithTracing(tracer =>
    {
        tracer
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddEntityFrameworkCoreInstrumentation()
            .AddSource("MainApp") // or "SearchService" depending on project
            .AddOtlpExporter(opt =>
            {
                opt.Endpoint = new Uri("http://localhost:4317");
            });
    })
    .WithMetrics(meter =>
    {
        meter.AddAspNetCoreInstrumentation();
        meter.AddRuntimeInstrumentation();
        meter.AddHttpClientInstrumentation();
    });
```

### Step 03: Aspire Integration

Aspire automatically configures an OpenTelemetry Collector that aggregates metrics and traces.
No additional collector setup is needed both MainApp and SearchService will send data through Aspire’s pipeline.

Run the Orchestrator and trigger requests across services.
You will see distributed traces showing the flow between MainApp and SearchService.
These traces can be visualized directly in Grafana if the **Tempo** data source is enabled.

---

## Task 03: Centralized Logging with Serilog and Elasticsearch

Logging is an essential part of observability.
We will configure structured, centralized logging using **Serilog** and **Elasticsearch**, and visualize logs in **Kibana**.

### Step 01: Add Elasticsearch and Kibana to Aspire

Modify your Orchestrator to include Elasticsearch and Kibana containers:

```cs
// Elasticsearch
var elastic = builder.AddContainer("elasticsearch", "docker.elastic.co/elasticsearch/elasticsearch:8.15.0")
    .WithEnvironment("discovery.type", "single-node")
    .WithEnvironment("xpack.security.enabled", "false")
    .WithPort(9200);

// Kibana
var kibana = builder.AddContainer("kibana", "docker.elastic.co/kibana/kibana:8.15.0")
    .WithEnvironment("ELASTICSEARCH_HOSTS", "http://elasticsearch:9200")
    .WithPort(5601);
```

After restarting Aspire, Elasticsearch will be available at `http://localhost:9200` and Kibana at [http://localhost:5601](http://localhost:5601).

---

### Step 02: Install and Configure Serilog

Install Serilog packages in both projects:

```bash
dotnet add package Serilog.AspNetCore
dotnet add package Serilog.Sinks.Console
dotnet add package Serilog.Sinks.Elasticsearch
```

Configure Serilog before building the application:

```cs
using Serilog;
using Serilog.Sinks.Elasticsearch;

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri("http://localhost:9200"))
    {
        AutoRegisterTemplate = true,
        IndexFormat = "seminar-logs-{0:yyyy.MM.dd}"
    })
    .CreateLogger();

builder.Host.UseSerilog();
```

Logs are automatically shipped to Elasticsearch and stored in daily indexes.

---

### Step 03: Validate Log Collection

Run the Orchestrator:

Open Kibana at [http://localhost:5601](http://localhost:5601) and select the `seminar-logs-*` index pattern under **Discover**.
Trigger actions in MainApp (e.g., create a product or execute a search query).
Structured log entries will appear in real time.

Example document in Elasticsearch:

```json
{
  "@timestamp": "2025-11-03T12:45:29.232Z",
  "level": "Information",
  "message": "Product created: ProductId=42, Name=\"Keyboard\"",
  "service": "MainApp",
  "traceId": "b7f29f9f3c4a4fd98c07aefc3cc47e42"
}
```

---

## Task 04: Verifying Full Observability

To validate all integrations, run the Orchestrator:

Check the following:

* **Grafana** → [http://localhost:3000](http://localhost:3000): Application metrics from MainApp and SearchService.
* **Kibana** → [http://localhost:5601](http://localhost:5601): Centralized logs with full-text search.
* **Aspire Dashboard** → [http://localhost:18888](http://localhost:18888): Health and topology of all services.
* **Traces (Grafana/Tempo)** → End-to-end request tracing across both services.

With these configurations, your Aspire environment now provides a complete observability suite:

* **Metrics** collected through Prometheus and Grafana
* **Traces** via OpenTelemetry
* **Logs** centralized in Elasticsearch
