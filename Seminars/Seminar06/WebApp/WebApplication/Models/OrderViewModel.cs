using BL.DTOs;

namespace PV293WebApplication.Models;

public class OrderViewModel
{
    public int Id { get; set; }

    public decimal Total { get; set; }

    public string FullName { get; set; } = default!;

    public string Address1 { get; set; } = default!;

    public string? Address2 { get; set; }

    public string City { get; set; } = default!;

    public string Zip { get; set; } = default!;

    public string Country { get; set; } = default!;

    public List<OrderItemDTO> Items { get; set; } = [];
}
