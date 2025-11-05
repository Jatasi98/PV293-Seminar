# Seminar 08 – From Monolith to Modulith and Messaging with RabbitMQ and Wolverine

## Initial Setup

In today’s seminar, we will continue building on top of the **Vertical Slice Architecture** solution from last week. Our goal is to transform our monolithic application into a **modulith**, introducing clear module boundaries and messaging between them.

If you were absent during the previous session, please download the prepared starting solution here:
[Vertical Slice Architecture – Solution](TODO)

## Task 01: Transforming the Monolith into a Modulith

In the previous seminar, our application followed the **Vertical Slice Architecture**, where all features were organized around specific use cases within a single monolithic solution. At that stage, we had 'modules' like **Products** and **Orders** tightly coupled under one host. For this seminar, we will assume that the **Orders functionality was removed** from the monolith to prepare it for extraction as an independent module.

Now, we will modularize the solution by splitting it into independent **feature modules** that can evolve, deploy, or even scale separately later on.

### Step 1: Introducing the Orders Module

We will reintroduce the **Orders** functionality as an independent **module**. A module should be self-contained, owning its data, logic, and API contracts. However, during this first step, we will **reuse our existing `DbContext`** from the current solution. The module will not be fully independent yet, but it will allow us to decouple its persistence later gradually.

Create a new project in your solution called 'Module.Orders'

Then, structure could be as:

```
Module.Orders/
│
├── Application/
│   ├── Features/
│   │   ├── CreateOrder/
│   │   ├── GetOrder/
│   │   └── ...
│   └── Behaviors/
│
├── Domain/
│   ├── Entities/
│   └── Events/
│
└── Infrastructure/
```

### Step 2: Integrating the Module into the Host Application

In the main API project, reference the Orders module.

Next, define a command that represents the creation of an order. Since the cart is currently held in the **session context**, we will include all cart items as part of the command payload.

**CreateOrderCommand.cs**

```csharp
public sealed record CreateOrderCommand(
    Customer Customer,
    List<CartItemDTO> Items
) : IRequest<int>;

public sealed record CartItemDTO(
    int ProductId,
    string ProductName,
    int Quantity,
    decimal UnitPrice
);
```

This command will be passed from the controller of the main e-commerce app to the application layer of the module. The handler will take these items, validate them, and persist the order using the shared DbContext.

Next, expose the Orders module through a controller, for example, `OrdersController.cs` inside your web project:

```csharp
[ApiController]
[Route("api/[controller]")]
public sealed class OrdersController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpPost]
    public async Task<IActionResult> CreateOrder()
    {
        var command = new CreateOrderCommand()
        {
            // Load data using GetCart from session
        };

        var id = await _mediator.Send(command);
        return Ok(new { OrderId = id });
    }
}
```

At this point, your Orders functionality is encapsulated within its own module, but it still runs within the same process, which means it is now a **modulith**.

## Task 02: Messaging and RabbitMQ

Before introducing `Wolverine`, we will first explore **messaging concepts** using **RabbitMQ** directly. This will help you understand how message queues work internally.

### Step 1: Running RabbitMQ

If you have Docker installed, start a local RabbitMQ instance:

```bash
docker run -d --hostname rabbitmq --name seminar-rabbit -p 5672:5672 -p 15672:15672 rabbitmq:3-management
```

You can now open the RabbitMQ Management UI at:
[http://localhost:15672](http://localhost:15672)
Default credentials: **guest / guest**

### Step 2: Sending and Receiving a Message

We will add a simple demonstration of sending and receiving a message directly through RabbitMQ. For that we will need to install RabbitMQ nuget for .Net.

```bash
dotnet add package RabbitMQ.Client
```

Add the following API controller to your main application:

```csharp
[ApiController]
[Route("api/messaging")]
public sealed class MessagingController : ControllerBase
{
    [HttpPost("send")]
    public async Task<IActionResult> SendMessage(string text)
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        using var connection = await factory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();

        await channel.QueueDeclareAsync(queue: "seminar-queue",
            durable: false, exclusive: false, autoDelete: false, arguments: null);

        var body = Encoding.UTF8.GetBytes(text);

        await channel.BasicPublishAsync(string.Empty, "seminar-queue", false , body);

        return Ok($"Message '{text}' sent to queue.");
    }
}
```

Then, add a simple console or background task in the same project that consumes the messages:

```csharp
var factory = new ConnectionFactory() { HostName = "localhost" };
using var connection = await factory.CreateConnectionAsync();
using var channel = await connection.CreateChannelAsync();

await channel.QueueDeclareAsync(queue: "seminar-queue", durable: false, exclusive: false, autoDelete: false, arguments: null);

var consumer = new AsyncEventingBasicConsumer(channel);
consumer.ReceivedAsync += async (model, ea) =>
{
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);

    Console.WriteLine($"Received: {message}");

    await channel.BasicAckAsync(ea.DeliveryTag, false);
};

await channel.BasicConsumeAsync(queue: "seminar-queue", autoAck: true, consumer: consumer);
Console.ReadLine();
```

You now have a working example of sending and receiving messages using RabbitMQ manually.

## Task 03: Introducing Wolverine

Now that you understand how RabbitMQ works, we will introduce **Wolverine** as a messaging library that simplifies RabbitMQ integration, automatically handling queues, message discovery, and reliability.

We will use the same running Docker RabbitMQ instance from the previous step.

### Step 1: Installing Wolverine

Add Wolverine to your main Web project:

```bash
dotnet add package Wolverine
dotnet add package Wolverine.RabbitMQ
dotnet add package Wolverine.FluentValidation
```

### Step 2: Configuring Wolverine

Modify your `Program.cs` to register Wolverine:

```csharp
builder.Host.UseWolverine(opts =>
{
    opts.UseRabbitMq(new Uri("amqp://guest:guest@localhost:5672"))
        .AutoProvision()
        .AutoPurgeOnStartup();

    opts.Discovery.IncludeAssembly(typeof(Program).Assembly);
});
```

### Step 3: Handling Messages in the Main Application

Since we do not yet have another independent module, we will handle messages within our main application.

Define a message and handler:

```csharp
public sealed record SeminarMessage(string Text);

public sealed class SeminarMessageHandler
{
    private readonly ILogger<SeminarMessageHandler> _logger;

    public SeminarMessageHandler(ILogger<SeminarMessageHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(SeminarMessage message)
    {
        _logger.LogInformation("Received message through Wolverine: {Text}", message.Text);
        return Task.CompletedTask;
    }
}
```

You can then publish this message from a controller:

```csharp
[ApiController]
[Route("api/wolverine")]
public sealed class WolverineController : ControllerBase
{
    private readonly IMessageBus _bus;

    public WolverineController(IMessageBus bus)
    {
        _bus = bus;
    }

    [HttpPost("send")]
    public async Task<IActionResult> SendWithWolverine(string text)
    {
        await _bus.PublishAsync(new SeminarMessage(text));
        return Ok($"Sent Wolverine message: {text}");
    }
}
```

At this stage, Wolverine automatically manages connections, exchanges, and message routing, providing a much cleaner approach than manual RabbitMQ integration.