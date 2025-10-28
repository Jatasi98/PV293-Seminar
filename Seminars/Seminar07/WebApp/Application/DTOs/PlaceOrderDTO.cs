namespace Application.DTOs;

public class PlaceOrderDTO
{
    public string FullName { get; set; } = string.Empty;

    public string Address1 { get; set; } = string.Empty;

    public string? Address2 { get; set; }

    public string City { get; set; } = string.Empty;

    public string Zip { get; set; } = string.Empty;

    public string Country { get; set; } = string.Empty;

    public CartDTO Cart { get; set; } = new();
}
