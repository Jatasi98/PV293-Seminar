using BL.DTOs;
using BL.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PV293WebApplication.Models;

namespace PV293WebApplication.Controllers.Admin;

[Area("Admin")]
public class ProductsController : AdminController
{
    private readonly IProductService _productService;
    private readonly ICategoryService _categoryService;

    public ProductsController(IProductService productService, ICategoryService categoryService)
    {
        _productService = productService;
        _categoryService = categoryService;
    }

    public async Task<IActionResult> Index()
    {
        var products = await _productService.GetProducts();
        return View(products);
    }

    public async Task<IActionResult> Details(int id)
    {
        var product = await _productService.FindProductById(id);
        if (product == null)
        {
            return NotFound();
        }

        return View(product);
    }

    public async Task<IActionResult> Create()
    {
        await TryFillCategoriesDropDown();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateProductViewModel model)
    {
        if (ModelState.IsValid)
        {
            var product = new ProductDTO
            {
                Name = model.Name,
                Price = model.Price,
                Description = model.Description,
                IsDeleted = model.IsDeleted,
                CategoryId = model.CategoryId
            };

            await _productService.CreateProduct(product);
            return RedirectToAction(nameof(Index));
        }

        await TryFillCategoriesDropDown(model.CategoryId);
        return View(model);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var product = await _productService.FindProductById(id);
        if (product == null)
        {
            return NotFound();
        }

        await TryFillCategoriesDropDown(product.CategoryId);

        var model = new UpdateProductViewModel
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            Description = product.Description,
            IsDeleted = product.IsDeleted,
            CategoryId = product.CategoryId
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, UpdateProductViewModel model)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        if (ModelState.IsValid)
        {
            var product = new ProductDTO
            {
                Id = model.Id,
                Name = model.Name,
                Price = model.Price,
                Description = model.Description,
                IsDeleted = model.IsDeleted,
                CategoryId = model.CategoryId
            };

            await _productService.UpdateProduct(product);
            return RedirectToAction(nameof(Index));
        }

        await TryFillCategoriesDropDown(model.CategoryId);
        return View(model);
    }

    private async Task TryFillCategoriesDropDown(int? selectedCategoryId = null)
    {
        var categories = await _categoryService.GetCategories();
        ViewBag.Categories = new SelectList(categories, "Id", "Name", selectedCategoryId);
    }
}
