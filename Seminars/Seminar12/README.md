# Seminar 12 – Event-Driven Architecture (Event Sourcing and State Reconstruction)

In this seminar, we will extend our existing system by introducing **Event Sourcing principles**.
Rather than persisting only the current state of our entities, we will capture each change as an immutable **domain event** and store it in an **event store**.
When the system restarts or recovers from a failure, we can replay these stored events to **reconstruct the previous system state**.
This provides complete traceability and ensures resilience through recoverable domain state.

---

## Introducing Event Sourcing

In traditional systems, the database stores only the *latest state* of entities, such as the current contents of a cart.
In contrast, **event-sourced systems** store *all the events that led to that state*. Each event represents a fact that occurred in the domain and is immutable.

**Example:**

```
1. ProductAddedToCart(ProductId: 101)
2. ProductAddedToCart(ProductId: 102)
3. ProductRemovedFromCart(ProductId: 101)
```

By replaying these events, the system derives the current cart state (which contains only Product 102).

Event Sourcing makes it possible to:

* Rebuild any aggregate from its event history.
* Provide a full audit trail of changes.
* Enable system recovery after crashes or migrations.

---

## Task 01: Creating Event Models and Event Store

We begin by defining domain events and implementing a simple **event store** to persist them.

### Step 01: Define Domain Events

```cs
public interface IDomainEvent
{
    DateTime OccurredOn { get; }
}

public sealed record ProductAddedToCart(Guid CartId, Guid ProductId) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public sealed record ProductRemovedFromCart(Guid CartId, Guid ProductId) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
```

### Step 02: Event Store Interface

```cs
public interface IEventStore
{
    Task AppendAsync(/* TODO */);
    Task<IEnumerable<IDomainEvent>> LoadAsync(/* TODO */);
}
```

### Step 03: Implement Database-Backed Event Store

Here is a code skeleton that you will need to update. You need to create a new entity, StoredEvent, which will be used as the database entity for persisting our processed domain events. Think about everything that is required to store our event and how you would retrieve it from the database.

```cs
public class EventStore(WebAppDbContext db) : IEventStore
{
    private readonly WebAppDbContext _db = db;

    public async Task AppendAsync(/* TODO */)
    {
        var entry = new StoredEvent
        {
            // TODO: Create the StoredEvent entity and map all required fields for persistence
        };

        _db.Events.Add(entry);
        await _db.SaveChangesAsync(ct);
    }

    public async Task<IEnumerable<IDomainEvent>> LoadAsync(/* TODO */)
    {
        var entries = // TODO: Fetch the required domain events

        return entries;
    }
}
```

---

## Task 02: Replaying Events to Rebuild Application State

In an event-sourced system, the current state of an entity (or aggregate) is not stored directly. Instead, the system persists events that describe the changes that occurred over time. Whenever the application needs the current state, it can reconstruct that state by sequentially replaying the stored events.

This process is known as rehydration or event replaying.

### Step 01: Create the Aggregate Root

```cs
public sealed class Cart : AggregateRoot
{
    private readonly List<Guid> _items = new();

    public IReadOnlyCollection<Guid> Items => _items;

    // TODO: Implement logic to rehydrate the Cart aggregate from its DomainEvents
}
```

### Step 02: Rehydrate Aggregate from Events

```cs
var evets = await _eventStore.LoadAsync(cartId, cancellationToken);

var cartAggregate = new Cart();
cartAggregate.Rehydrate(evets);
```

After replaying all stored events, the cart will match its previous in-memory state.

---

## Task 03: Integrating Event Sourcing into Cart Handling

Modify the cart feature so that changes are applied by recording domain events instead of storing cart data in session or temporary state. Whenever a product is added or removed, an event should be appended. When the cart is queried, its state must be reconstructed by replaying stored events.

### Step 01: Update AddCartItemHandler

Refactor the handler so that adding an item to the cart is handled using event sourcing. It should append the appropriate domain events to the event store instead of relying on session-based or in-memory cart state.

### Step 02: Extend the Get Cart functionality

Update the GetCartController so that it uses a dedicated query and corresponding query handler. The handler should:
* Load the relevant domain events for the given cart
* Rehydrate the cart from those events
* Return the resulting cart state

### Step 03: Extend the Remove Cart Item functionality

Update the RemoveCartItemController so that it uses a dedicated command and corresponding command handler, similar to the Add Cart feature. The handler should:
* Append the appropriate domain event(s) representing item removal
* No longer depend on temporary session handling from CartControllerBase

The goal is that the cart feature is handled consistently via commands/queries and their handlers, all backed by event sourcing rather than session-based storage.

---

## Task 04: Testing Recovery and State Reconstruction

1. Start the application and perform actions such as:
   * Add items to the cart.
   * Remove one item.
2. Inspect the `Events` table in the database to verify that all domain events were recorded.
3. Stop the application.
4. Restart the app and fetch the cart using `GetCart()` — the previous cart state should be fully restored.
