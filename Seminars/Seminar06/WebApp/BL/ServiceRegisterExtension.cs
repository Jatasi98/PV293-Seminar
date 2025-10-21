using BL.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BL;

public static class ServiceRegisterExtension
{
    public static void RegisterServices(this IServiceCollection services)
    {
        services.AddTransient<IAccountService, AccountService>();
        services.AddTransient<ICatalogService, CatalogService>();
        services.AddTransient<ICheckoutService, CheckoutService>();
        services.AddTransient<IOrderService, OrderService>();
        services.AddTransient<IProductService, ProductService>();
        services.AddTransient<ISearchService, SearchService>();
    }
}
