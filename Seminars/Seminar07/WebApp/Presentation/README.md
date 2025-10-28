# Seminar 07 – Vertical Slice Architecture and CQRS

## Initial Setup

Today, we will continue with the task from last week. If you have already completed the transformation from N-Layered to clean architecture, please proceed with your existing solution. However, if you were absent during the previous session, you can find the prepared solution from last week at the following address: [Clean architecture - Solution](https://youtu.be/dQw4w9WgXcQ?si=qMCUud-LbLrWpUFs).

## Task 01: MediatR

We will focus on modifying our application to operate using the Mediator pattern. To achieve this, we will use the following library: [MediatR](https://github.com/LuckyPennySoftware/MediatR). 

Using the NuGet Package Console, we will install MediatR:

```bash
dotnet add package MediatR
dotnet add package MediatR.Extensions.Microsoft.DependencyInjection
```

Next, we will modify the Program.cs file to enable the use of the MediatR library.

```cs
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
```

If the installation is successful, we will refactor one request in our Clean Architecture application (either a Command or a Query) so that we no longer rely on direct calls through the business layer and its services. Before changing any existing code, please prepare the following components.

At the Application layer, create a `Features` folder. This folder will contain all our `features`/`use cases` for `Products`, `Orders`, etc. Each module folder will include the subfolders `Commands` and `Queries` (*Commands are for modifications, Queries are for data retrieval*). Depending on what you modify in your application, you will need a `Request` (Command or Query) with a corresponding `Handler`. An example of what these elements might look like:

### AddProductCommand.cs

```cs
public sealed record AddProductCommand(string Name, decimal Price) : IRequest<Guid>;
```

### AddProductHandler.cs

```cs
public sealed class AddProductHandler : IRequestHandler<AddProductCommand, Guid>
{
    // Injected services, repositories... what we need to process the request

    public AddProductHandler()
    {
        // DI
    }

    public async Task<Guid> Handle(AddProductCommand request, CancellationToken ct)
    {
        var product = new Product(request.Name, request.Price);

        var createdProductId = await _productService.AddNewProduct(product, ct);.

        return createdProductId;
    }
}
```

Now that we have prepared our `Request` and `Handler`, let us apply them in the `Presentation layer`. In the controller responsible for adding new products, the corresponding method should be added or refactored as follows.

### ProductAdminController.cs

```cs

public ProductAdminController : AdminController
{
    private readonly IMediator _mediator;

    public ProductAdminController(IMediator mediator)
    {
        _mediator = mediator;
    }


    [HttpPost]
    public async Task<IActionResult> AddProduct(AddProductCommand command)
    {
        var id = await _mediator.Send(command);
        
        // Return what you want - redirect to the newly created product or raise a notification for a user with information about the newly created product.
    }
}
```

If you have completed all the steps correctly, the application should still run without any noticeable changes from the user’s perspective. Thanks to this refactoring, you have cleanly separated the Presentation layer from the business logic.

Your task is to modify a part of your existing application to use MediatR. You may either reintroduce the `Create Product` functionality into our application or select another feature and implement it with MediatR. Ensure that everything remains fully functional after your changes.

## Task 02: Validators in MediatR

Last time, we created a validator not bound to any command or query, requiring us to instantiate and invoke it manually. Now, we will implement a validator that runs automatically once defined, without any additional wiring on your part. First, ensure `FluentValidation` is installed in the project where you intend to use the validator. If it is not already present, install it via the NuGet Package Manager or the Package Manager Console.

```bash
dotnet add package FluentValidation
```

### AddProductCommandValidator.cs 

```cs
public sealed class AddBookCommandValidator : AbstractValidator<AddProductCommand>
{
    public AddBookCommandValidator()
    {
        RuleFor(x => x.Name)
            .Must(BeAlphanumeric).WithMessage("Name must be alphanumeric.")
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(200).WithMessage("Name must be <= 200 characters.");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than zero.")
            .ScalePrecision(2, 18).WithMessage("Price must have at most 2 decimal places.");
    }

    private static bool BeAlphanumeric(string title) =>
        title.All(ch => char.IsLetterOrDigit(ch));
}
```
To connect MediatR with our newly created validator, we need to register it. We will use `IPipelineBehavior` from the `MediatR` library for this purpose. In the `Application layer`, add `Common/Behaviors/ValidationBehavior.cs`. By leveraging generics, we can implement a validator that automatically executes all validators defined for that request upon intercepting any Command or Query.

### ValidationBehavior.cs

```cs
public sealed class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken ct)
    {
        if (!_validators.Any())
            return await next();

        var context = new ValidationContext<TRequest>(request);
        var errors = (await Task.WhenAll(
                _validators.Select(v => v.ValidateAsync(context, ct))))
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .ToList();

        if (errors.Count != 0)
        {
            var message = string.Join("; ", errors.Select(e => e.ErrorMessage));
            throw new ValidationException(message);
        }

        return await next();
    }
}
```

Before we verify that everything has been prepared correctly, you must register the ValidationBehavior in Program.cs. To simplify this registration, add the following class to the Application layer. It will help you obtain the required assembly.

### AssemblyMarker.cs

```cs
public sealed class AssemblyMarker { }
```

```cs
// Pro pouziti budete potrebovat nuget `FluentValidation.DependencyInjectionExtensions`
builder.Services.AddValidatorsFromAssembly(typeof(Application.AssemblyMarker).Assembly);

builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
```

On the next application run, when creating a product, the corresponding validator should be triggered, and validation should occur as the request passes through MediatR.

As an exercise, please implement at least one additional validator for a request of your choice.

## Task 03: Events

We already know from the previous session that domain events can exist. Now we will create one and publish it within our application so that we can capture and process it. For example, when a product is added as demonstrated above, we should log that this action occurred. To achieve this, we will prepare an event that, in MediatR, is represented by `INotification`.

### ProductCreatedEvent.cs

```cs
public sealed record ProductCreatedEvent(Guid ProductId) : INotification;
```

Next, in the part of the application where the product is created, we will make a modification that allows us to call the `Publish()` method.

```cs
    private readonly IMediator _mediator;

    public async Task<Guid> Handle(PlaceOrderCommand request, CancellationToken ct)
    {
        // Code...

        var event = new ProductCreatedEvent(productId);        
        await _mediator.Publish(event, ct);

        // Code...
    }
```

Finally, we need to capture `ProductCreatedEvent` event and process it. Prepare a handler which will log information about what product was created.

### ProductCreatedLoggingHandler.cs

```cs
public sealed class ProductCreatedLoggingHandler : INotificationHandler<ProductCreatedEvent>
{
    private readonly ILogger<ProductCreatedLoggingHandler> _logger;

    public ProductCreatedLoggingHandler(ILogger<ProductCreatedLoggingHandler> logger) => _logger = logger;

    public Task Handle(ProductCreatedEvent notification, CancellationToken ct)
    {
        _logger.LogInformation("Product created: {ProductId} at {DateTimeUTC}",
            notification.ProductId, DateTime.UtcNow);

        return Task.CompletedTask;
    }
}
```

Design one use case for which publishing an event in our system would be beneficial, and implement it on your own.

## Bonus: Vertical slice architecture

If you have completed all the tasks assigned in today's seminar, please focus on the application. At present, it is implemented to follow Clean Architecture principles. Refactor it to conform to Vertical Slice Architecture principles, as discussed during the in-class presentation.