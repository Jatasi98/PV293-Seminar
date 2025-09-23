using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication1.DAL;
using WebApplication1.Entities;

namespace WebApplication1.Controllers.Admin;

[Area("Admin")]
public class OrdersController : Controller
{
    private readonly BaseRepository _db;

    public OrdersController(BaseRepository db) => _db = db;

    public async Task<IActionResult> Index()
    {
        var orders = await _db.Orders
            .Include(o => o.Customer)
            .OrderByDescending(o => o.CreatedOnUTC)
            .ToListAsync();
        return View(orders);
    }

    public async Task<IActionResult> Details(int id)
    {
        var order = await _db.Orders
            .Include(o => o.Customer)
            .FirstOrDefaultAsync(o => o.Id == id);
        if (order == null) return NotFound();
        return View(order);
    }

    public IActionResult Create()
    {
        ViewBag.CustomerId = new SelectList(_db.Customers, "Id", "Email");
        return View(new Order { CreatedOnUTC = DateTime.UtcNow });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Order order)
    {
        if (ModelState.IsValid)
        {
            _db.Orders.Add(order);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        ViewBag.CustomerId = new SelectList(_db.Customers, "Id", "Email", order.CustomerId);
        return View(order);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var order = await _db.Orders.FindAsync(id);
        if (order == null) return NotFound();

        ViewBag.CustomerId = new SelectList(_db.Customers, "Id", "Email", order.CustomerId);
        return View(order);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Order order)
    {
        if (id != order.Id) return BadRequest();

        if (ModelState.IsValid)
        {
            try
            {
                _db.Update(order);
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _db.Orders.AnyAsync(o => o.Id == id)) return NotFound();
                throw;
            }
            return RedirectToAction(nameof(Index));
        }
        ViewBag.CustomerId = new SelectList(_db.Customers, "Id", "Email", order.CustomerId);
        return View(order);
    }

    public async Task<IActionResult> Delete(int id)
    {
        var order = await _db.Orders
            .Include(o => o.Customer)
            .FirstOrDefaultAsync(o => o.Id == id);
        if (order == null) return NotFound();
        return View(order);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var order = await _db.Orders.FindAsync(id);
        if (order != null)
        {
            _db.Orders.Remove(order);
            await _db.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}
