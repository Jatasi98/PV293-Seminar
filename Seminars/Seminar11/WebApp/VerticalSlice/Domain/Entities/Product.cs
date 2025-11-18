namespace VerticalSlice.Domain.Entities;

public class Product : BaseEntity
{
    public string Name { get; set; } = default!;

    public Money Price { get; set; } = default!;

    public string? Description { get; set; }

    public bool IsDeleted { get; set; } = false;

    public int? CategoryId { get; set; }

    public Category? Category { get; set; }

    public static Product Create(string name, string? description, Money price, int? categoryId)
    {
        return new()
        {
            Name = name,
            Description = description,
            Price = price,
            CategoryId = categoryId,
        };
    }

    public void UpdateDescription(string? description)
    {
        Description = description;
    }

    public void ChangeCategory(int? categoryId)
    {
        CategoryId = categoryId;
    }

    public void ChangePrice(Money newPrice)
    {
        Price = newPrice;
    }

    public void SoftDelete()
    {
        IsDeleted = true;
    }
}

public sealed class Money
{
    public decimal Value { get; set; }
    public string Currency { get; set; } = default!;

    public static Money Create(decimal value, string currency)
    {
        return new()
        {
            Value = value,
            Currency = currency,
        };
    }

    public override bool Equals(object? obj)
    {
        if (obj is not Money other)
        {
            return false;
        }

        return Value == other.Value
            && Currency == other.Currency;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Value, Currency);
    }
}
