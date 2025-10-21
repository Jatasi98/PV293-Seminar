# Seminar 06 – Clean Architecture

Today, we will focus on applying Clean Architecture principles to an existing monolithic three-layer application.

As a quick recap, look at the following table, which summarizes the main layers of Clean Architecture and their responsibilities.

| Layer | Depends on | Contains | Purpose |
|---------------------------|-------------|----------------|-----------|
| **Domain** | Nothing | Entities, Value Objects, Events, Exceptions | Represents the core business rules and logic. Defines how the business works, independent of frameworks, databases, or UI. |
| **Application** | Domain | Use Cases, Interfaces, DTOs, Validators | Defines what the system should do, not how it does it. Serves as the entry point for business operations. |
| **Infrastructure** | Application, Domain | Implementations of repositories, database access, services, file storage, third-party APIs | Provides the technical details and actual implementations for the abstractions defined in the Application layer. |
| **Presentation (Web / UI)** | Application | Controllers, Views, API Endpoints, UI Components, Request/Response Models, Dependency Injection configuration | Handles user interaction and input/output. |

We will create a clone of our existing application and transform it into a structure that follows the four fundamental layers of Clean Architecture.

For those who finish early there will be an additional challenge to enhance the solution by integrating the MediatR library.

---

## Initial Setup

Please download the latest changes from the repository at [Seminar Github](https://github.com/Jatasi98/PV293-Seminar)

The `Seminars` folder has a subfolder marked with the number of the current session.

It contains an almost complete three-layer application that was previously used during the second seminar, which focused on refactoring. Before making any modifications, it is recommended to examine the application, particularly the backend part, in its current state. The Presentation layer is no longer directly connected to the Data Access layer. 

> The remaining connections to the DAL are within the login handling logic and in Program.cs.

---

## Task 01: Clean architecture - Domain

The first part focuses on creating the Domain section of the application. As stated in the introduction, the Domain section should contain only: Entities, Value Objects, Events, and Exceptions.

The task is to prepare the domain layer and move the above-mentioned parts from the existing application into it.

Create a new solution next to the existing one. In this solution, create a class library named, for example, `WebEshop.Domain` and gradually create all required Entities and Value Objects. Ensure that the Domain layer remains free of Entity Framework attributes to maintain separation from persistence frameworks.

Example of an Entity:

```csharp
namespace Library.Domain.Entities
{
    public class Book
    {
        public Guid Id { get; private set; }
        public string Title { get; private set; }
        public string Author { get; private set; }
        public int AvailableCopies { get; private set; }

        public Book(string title, string author, int copiesCount)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title cannot be empty.");

            if (string.IsNullOrWhiteSpace(author))
                throw new ArgumentException("Author cannot be empty.");

            Id = Guid.NewGuid();
            Title = title;
            Author = author;
            AvailableCopies = copiesCount;
        }

        public void Borrow()
        {
            if (AvailableCopies <= 0)
                throw new InvalidOperationException("No copies available for borrowing.");

            AvailableCopies--;
        }

        public void Return()
        {
            AvailableCopies++;
        }
    }
}
```

If it were necessary to add a price to a book (e.g. for situations where a borrower loses it) a Value Object can represent and store the book’s price.

Example of such a Value Object:

```csharp
namespace Library.Domain.ValueObjects
{
    public sealed class Money : IEquatable<Money>
    {
        public decimal Amount { get; }
        public string Currency { get; }

        public Money(decimal amount, string currency)
        {
            if (amount < 0)
                throw new ArgumentException("Amount cannot be negative.");

            if (string.IsNullOrWhiteSpace(currency))
                throw new ArgumentException("Currency must be provided.");

            Amount = decimal.Round(amount, 2, MidpointRounding.AwayFromZero);
            Currency = currency.ToUpperInvariant();
        }

        public Money Add(Money other)
        {
            EnsureSameCurrency(other);

            return new Money(Amount + other.Amount, Currency);
        }

        public Money Subtract(Money other)
        {
            EnsureSameCurrency(other);

            if (Amount < other.Amount)
                throw new InvalidOperationException("Insufficient amount to subtract.");

            return new Money(Amount - other.Amount, Currency);
        }

        private void EnsureSameCurrency(Money other)
        {
            if (Currency != other.Currency)
                throw new InvalidOperationException("Currency mismatch.");
        }

        public override string ToString() => $"{Amount:N2} {Currency}";

        public bool Equals(Money? other)
        {
            if (other is null)
                return false;

            return Amount == other.Amount && Currency == other.Currency;
        }

        public override bool Equals(object? obj) => Equals(obj as Money);
        public override int GetHashCode() => HashCode.Combine(Amount, Currency);

        public static bool operator ==(Money left, Money right) => Equals(left, right);
        public static bool operator !=(Money left, Money right) => !Equals(left, right);
    }
}
```

If desired, custom exceptions can be implemented here. They can replace existing exceptions, such as ArgumentException, with MoneyNegativeException. This enables more precise handling of errors in the upper layers of the application.

As for events, these will be explored if the bonus section is reached today. Otherwise, events will be covered thoroughly in upcoming sessions.

---

## Task 02: Clean architecture - Application

In the following section, the focus shifts to the application layer. This layer will concentrate on interfaces and DTOs and will also include preparing validators. 

> The Use Case implementation will appear only in the bonus part of the exercise.

The application layer specifies only what capabilities should be available for the application. For this reason, this layer will hold all interfaces, which will then be implemented in the infrastructure layer.

All DTOs should also be defined here so that domain entities are not sent outside the core.

Begin by creating the library `WebEshop.Application` and move into it all items mentioned above that belong to this layer.

Prepare at least one custom validator in the application layer. Use the library [FluentValidation](https://github.com/FluentValidation/FluentValidation).

```csharp
public sealed class AddBookCommandValidator : AbstractValidator<AddBookCommand>
{
    public AddBookCommandValidator()
    {
        RuleFor(x => x.Title)
            .Must(BeAlphanumeric).WithMessage("ISBN must be alphanumeric.") // Custom validator
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title must be <= 200 characters.");

        RuleFor(x => x.Author)
            .NotEmpty().WithMessage("Author is required.")
            .MaximumLength(120).WithMessage("Author must be <= 120 characters.");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than zero.")
            .ScalePrecision(2, 18).WithMessage("Price must have at most 2 decimal places.");

        RuleFor(x => x.Currency)
            .NotEmpty().WithMessage("Currency is required.")
            .Must(c => c is "USD" or "EUR" or "GBP").WithMessage("Unsupported currency.");
    }

    private static bool BeAlphanumeric(string title) =>
        title.All(ch => char.IsLetterOrDigit(ch));
}
```

---

## Task 03: Clean architecture - Infrastructure

Now the focus turns to implementing the interfaces defined in the application layer. This is also where persistence occurs, meaning this layer is responsible for choosing and integrating the database technology.

Create a library named `WebEshop.Infrastructure` and gradually move all necessary interface implementations from the existing application into it.

---

## Task 04: Clean architecture - Presentation + DI wiring

The final layer remaining to complete the Clean Architecture application is the Presentation layer. Gradually add Controllers, Views, and their models into this layer.

Create an ASP.NET MVC project named `WebEshop.Presentation` and try to connect in to the rest of our new application.

During the presentation the approach to wiring and deploying dependency injection in a Clean Architecture application was disccused. The final step for today is to bring the application to life by connecting dependency injection and starting the application.

---

## Bonus: MediatR

Those with no remaining tasks can attempt to integrate the MediatR library into the web application.

First, review the pages at [MediatR](https://github.com/LuckyPennySoftware/MediatR).

Afterwards, select one of the controllers and refactor its actions to use the previously mentioned library for handling requests.