namespace BL.DTOs;

public class PlaceOrderDTO
{
    public string FullName { get; set; } = "";

    public string Address1 { get; set; } = "";

    public string? Address2 { get; set; }

    public string City { get; set; } = "";

    public string Zip { get; set; } = "";

    public string Country { get; set; } = "";

    public CartDTO Cart { get; set; } = new();
}
