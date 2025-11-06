using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Module.Orders.Infrastructure;
using VerticalSlice.Infrastructure;
using VerticalSlice.Middlewares;
using VerticalSlice.PipleBehaviors;
using VerticalSlice.Services;
using Wolverine;
using Wolverine.RabbitMQ;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// Migration:
// 1. dotnet ef migrations add InitMainDb --context WebAppDbContext --project VerticalSlice --startup-project VerticalSlice -o Migrations
// 2. dotnet ef database update --context WebAppDbContext --project VerticalSlice --startup-project VerticalSlice
builder.Services.AddDbContext<WebAppDbContext>(options =>
            options.UseSqlite(builder.Configuration.GetConnectionString("Default") ?? throw new ArgumentException("Connection string is needed to run the application")));

// Migration:
// 1. dotnet ef migrations add InitOrdersDb --context OrderDbContext --project Module.Orders --startup-project VerticalSlice -o Migrations
// 2. dotnet ef database update --context OrderDbContext --project Module.Orders --startup-project VerticalSlice
builder.Services.AddDbContext<OrderDbContext>(options =>
            options.UseSqlite(builder.Configuration.GetConnectionString("Orders") ?? throw new ArgumentException("Connection string is needed to run the application")));

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(12);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies()));

builder.Services.AddValidatorsFromAssemblies(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

builder.Services.AddTransient<ICategoryService, CategoryService>();
builder.Services.AddTransient<IProductService, ProductService>();
builder.Services.AddTransient<ISearchService, SearchService>();

builder.Host.UseWolverine(opts =>
{
    opts.UseRabbitMq(new Uri("amqp://guest:guest@localhost:5672"))
        .AutoProvision()
        .AutoPurgeOnStartup();

    opts.Discovery.IncludeAssembly(typeof(Program).Assembly);
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseSession();

app.UseLoggingMiddleware();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
