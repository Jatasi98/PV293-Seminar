using Application.Services.Interfaces;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class ServiceRegisterExtension
{
    public static void AddDbContext(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<WebAppDbContext>(options =>
            options.UseSqlite(connectionString));
    }

    public static void RegisterServices(this IServiceCollection services)
    {
        services.AddTransient<ICatalogService, CatalogService>();
        services.AddTransient<ICheckoutService, CheckoutService>();
        services.AddTransient<IOrderService, OrderService>();
        services.AddTransient<IProductService, ProductService>();
        services.AddTransient<ISearchService, SearchService>();
    }
}
