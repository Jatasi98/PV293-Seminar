using Application.DTOs;
using Application.Services.Interfaces;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Presentation.Helpers;
using Presentation.Models;

namespace Presentation.Controllers;

public class CheckoutController : Controller
{
    private readonly ICheckoutService _checkoutService;

    public CheckoutController(ICheckoutService checkoutService)
    {
        _checkoutService = checkoutService;
    }

    [HttpGet]
    public IActionResult Index()
    {
        var cart = HttpContext.Session.GetObject<CartViewModel>(Constants.CartKey)
            ?? new CartViewModel();

        if (cart.Items.Count == 0)
        {
            return RedirectToAction("Index", "Cart");
        }

        var model = new CheckoutViewModel { Cart = cart };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> PlaceOrder(CheckoutViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            return View("Index", vm);
        }

        var newOrder = new PlaceOrderDTO()
        {
            Address1 = vm.Address1,
            Address2 = vm.Address2,
            Cart = new() { Items = vm.Cart.Items },
            City = vm.City,
            Country = vm.Country,
            FullName = vm.FullName,
            Zip = vm.Zip,
        };

        var customer = new CustomerDTO()
        {

        };

        var createdOrderID = await _checkoutService.PlaceOrder(newOrder, customer);

        if (createdOrderID != null)
        {
            HttpContext.Session.Remove(Constants.CartKey);
        }

        return RedirectToAction(nameof(Confirmation), new { id = createdOrderID });
    }

    [HttpGet]
    public async Task<IActionResult> Confirmation(int id)
    {
        var createdOrder = await _checkoutService.Confirmation(id);

        if (createdOrder == null)
        {
            return NotFound();
        }

        var orderViewModel = new OrderViewModel()
        {
            Id = createdOrder.Id,
            Total = createdOrder.Total,
            FullName = createdOrder.FullName,
            Address1 = createdOrder.Address1,
            Address2 = createdOrder.Address2,
            City = createdOrder.City,
            Zip = createdOrder.Zip,
            Country = createdOrder.Country
        };

        return View(orderViewModel);
    }
}
