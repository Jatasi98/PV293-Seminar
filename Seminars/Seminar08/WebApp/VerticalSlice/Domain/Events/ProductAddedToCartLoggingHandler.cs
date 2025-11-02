using MediatR;

namespace VerticalSlice.Domain.Events;

public class ProductAddedToCartLoggingHandler(ILogger<ProductAddedToCartLoggingHandler> logger) : INotificationHandler<ProductAddedToCartEvent>
{
    private readonly ILogger<ProductAddedToCartLoggingHandler> logger = logger;

    public Task Handle(ProductAddedToCartEvent notification, CancellationToken ct)
    {
        logger.LogInformation("Product added to Cart: {ProductId} at {DateTimeUTC}",
            notification.ProductId, DateTime.UtcNow);

        return Task.CompletedTask;
    }
}
