using BL.DTOs;
using BL.Services;
using Microsoft.AspNetCore.Mvc;
using PV293WebApplication.Models;

namespace PV293WebApplication.Controllers.Admin;

[Area("Admin")]
public class OrdersController : AdminController
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService) => _orderService = orderService;

    public async Task<IActionResult> Index()
    {
        var orders = await _orderService.GetOrders();
        return View(orders);
    }

    public async Task<IActionResult> Details(int id)
    {
        var order = await _orderService.FindOrderById(id);
        if (order == null)
        {
            return NotFound();
        }

        return View(order);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var order = await _orderService.FindOrderById(id);
        if (order == null)
        {
            return NotFound();
        }

        return View(order);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateOrderViewModel model)
    {
        if (model.Id < 1)
        {
            return BadRequest();
        }

        if (ModelState.IsValid)
        {
            var order = new OrderDTO()
            {
                Id = model.Id,
                Total = model.Total,
                FullName = model.FullName,
                Address1 = model.Address1,
                Address2 = model.Address2,
                City = model.City,
                Zip = model.Zip,
                Country = model.Country,
                CustomerId = model.CustomerId
            };

            await _orderService.UpdateOrder(order);
            return RedirectToAction(nameof(Index));
        }

        return View(model);
    }
}
