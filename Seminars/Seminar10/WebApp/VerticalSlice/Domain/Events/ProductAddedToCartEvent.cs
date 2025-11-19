using MediatR;

namespace VerticalSlice.Domain.Events;

public sealed record ProductAddedToCartEvent(int ProductId) : INotification;