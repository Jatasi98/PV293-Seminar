using Application.DTOs;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[Authorize]
public class OrderController : Controller
{
    private readonly IOrderService _orderService;

    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    public async Task<IActionResult> Index()
    {
        var customer = new CustomerDTO()
        {

        };

        var orders = await _orderService.GetOrders(customer);

        if (orders == null)
        {
            return View(new List<OrderDTO>());
        }

        return View(orders);
    }

    public async Task<IActionResult> Details(int id)
    {
        var customer = new CustomerDTO()
        {

        };

        var order = await _orderService.GetOrder(customer, id);

        return View(order);
    }
}
