# Seminar 11 – Observability and Telemetry

In this seminar, we will extend our existing Aspire-based setup by introducing **observability and telemetry** to our distributed application.
We will configure **Grafana** for monitoring metrics, integrate **OpenTelemetry** for distributed tracing, and implement centralized **structured logging** using **Serilog** and **Elasticsearch**.
This configuration will provide complete visibility into system health, performance, and request flow across all running services.

---

## Task 01: OpenTelemetry Integration

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
```

### Step 02: Configure OpenTelemetry in Program.cs

In each project, register and configure tracing and metrics prior to building the application.

**WebApp - VerticalSlice**

```cs
// Extend UseWolverine with following
builder.Host.UseWolverine(opts =>
{
    opts.ServiceName = "web-app";

    //...
}

// OpenTelemtry
builder.Services.AddOpenTelemetry()
    .WithMetrics(m =>
    {
        m.AddAspNetCoreInstrumentation();
        m.AddMeter("Microsoft.AspNetCore.Hosting");
        m.AddMeter("Microsoft.AspNetCore.Server.Kestrel");
        m.AddMeter("System.Net.Http");
    })
    .WithTracing(t =>
    {
        t.AddSource("Wolverine");

        t.AddAspNetCoreInstrumentation();
        t.AddHttpClientInstrumentation();
    })
    .ConfigureResource(rb => rb.AddService("web-app"));

builder.Services.Configure<OpenTelemetryLoggerOptions>(x =>
{
    x.AddOtlpExporter();
});

builder.Services.ConfigureOpenTelemetryMeterProvider(x =>
{
    x.AddOtlpExporter();
});

builder.Services.ConfigureOpenTelemetryTracerProvider(x =>
{
    x.AddOtlpExporter();
});
```

**SearchService**

For the SearchService project, enable MongoDB tracking by installing and configuring the following NuGet package.

```bash
dotnet add package MongoDB.Driver.Core.Extensions.DiagnosticSources
```

```cs
// IMongoClient
builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var opts = sp.GetRequiredService<IOptions<MongoOptions>>().Value;

    var settings = MongoClientSettings.FromConnectionString(opts.ConnectionString);

    settings.ClusterConfigurator = cb =>
        cb.Subscribe(new DiagnosticsActivityEventSubscriber());

    return new MongoClient(settings);
});


builder.Host.UseWolverine(opts =>
{
    opts.ServiceName = "search-service";

    //...
}

// OpenTelemetry
builder.Services.AddOpenTelemetry()
    .WithMetrics(m =>
    {
        m.AddAspNetCoreInstrumentation();
        m.AddMeter("Microsoft.AspNetCore.Hosting");
        m.AddMeter("Microsoft.AspNetCore.Server.Kestrel");
        m.AddMeter("System.Net.Http");
    })
    .WithTracing(t =>
    {
        t.AddSource("MongoDB.Driver.Core.Extensions.DiagnosticSources");
        t.AddSource("Wolverine");

        t.AddAspNetCoreInstrumentation();
        t.AddHttpClientInstrumentation();
    })
    .ConfigureResource(rb => rb.AddService("search-service"));

builder.Services.Configure<OpenTelemetryLoggerOptions>(x =>
{
    x.AddOtlpExporter();
});

builder.Services.ConfigureOpenTelemetryMeterProvider(x =>
{
    x.AddOtlpExporter();
});

builder.Services.ConfigureOpenTelemetryTracerProvider(x =>
{
    x.AddOtlpExporter();
});
```

Validate that all incoming requests are observable within the application and that their end-to-end traversal through the solution can be traced.

## Task 02: Prometheus and Grafana

First, add a new NuGet package that enables data exposure for Prometheus. For now, focus only on the VerticalSlice component of the application.

```bash
dotnet add package OpenTelemetry.Exporter.Prometheus.AspNetCore
```

Also, create the following file in the AspireHost project.

**prometheus.yml**
```yml
scrape_configs:
  - job_name: scrap-service
    scrape_interval: 5s
    static_configs:
      - targets: [host.docker.internal:5271]
```

Finally, update the AppHost.cs file with the following changes.

```cs
builder.AddContainer(name: "prometheus", image: "prom/prometheus:v2.54.1")
    .WithBindMount("./prometheus.yml", "/etc/prometheus/prometheus.yml")
    .WithEndpoint(targetPort: 9090, port: 9090, scheme: "http");
```

Launch the application and inspect the exported metrics in the Prometheus UI. Summarize the insights and actions now enabled by the collected data.

Utilize Grafana, a monitoring and visualization tool, to track application metrics in real-time. Using Aspire, provision and configure Grafana automatically alongside the existing services. 

Configure Grafana to use Aspire’s built-in Prometheus endpoint as a data source, leveraging metrics collected from all running applications.

### Step 01: Add Grafana to the Aspire Orchestrator

Open the Orchestrator project and modify the `AppHost.cs` file to include a Grafana container.

```cs
// Add Grafana
builder.AddContainer("grafana", "grafana/grafana:11.2.0")
    .WithEndpoint(targetPort: 3000, port: 3000, scheme: "http")
    .WithEnvironment("GF_SECURITY_ADMIN_USER", "admin")
    .WithEnvironment("GF_SECURITY_ADMIN_PASSWORD", "admin")
    .WithEnvironment("GF_USERS_ALLOW_SIGN_UP", "false");
```

Once started, access Grafana at [http://localhost:3000](http://localhost:3000) with credentials:

```
Username: admin
Password: admin
```

Aspire provides a Prometheus-compatible endpoint by default; however, we now need to configure Grafana to locate and access data from Prometheus.

Once Grafana is running, navigate to `Connections` and select `Prometheus`. In the connection settings, enter `http://prometheus:9090`, then click `Save & test` at the bottom of the page.

Next, add your first dashboard:
Open `Dashboards`, choose `Import`, and in the `dashboard ID` field, enter 19924, then click `Load`. In the final step, select the Prometheus connection you created earlier and click `Import`.

After completing these steps, the dashboard will display various metrics and information. 

Take some time to explore and analyze the data presented.

---

## Task 03: Verifying Full Observability

To validate all integrations, run the Orchestrator:

Check the following:

* **Grafana** → [http://localhost:3000](http://localhost:3000): Application metrics from WebApp.
* **Aspire Dashboard** → [http://localhost:18888](http://localhost:18888): Health and topology of all services and End-to-end request tracing across both services.

With these configurations, your Aspire environment now provides a complete observability suite:

* **Metrics** collected through Prometheus and Grafana
* **Traces** via OpenTelemetry
