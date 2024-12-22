using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using System.Diagnostics;

public class ProductsController : Controller
{
    private readonly IProductsService _service;

    public ProductsController(IProductsService productsService)
    {
        _service = productsService;
    }

    // GET: Products/
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var productDTOs = await _service.GetProductsAsync();
        var productViewModels = productDTOs.Select(dto => new ProductViewModel
        {
            Id = dto.Id,
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price
        });

        return View(productViewModels);
    }

    // GET: Products/Delete/5
    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var productDTO = await _service.GetProductByIdAsync(id); 
        if (productDTO == null)
        {
            return NotFound();
        }

        var productViewModel = new ProductViewModel
        {
            Id = productDTO.Id,
            Name = productDTO.Name,
            Description = productDTO.Description,
            Price = productDTO.Price
        };

        return View(productViewModel);
    }

    // POST: Products/Delete/5
    [HttpPost, ActionName("DeleteConfirmed")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _service.DeleteProductAsync(id);
        return RedirectToAction(nameof(Index));
    }

}
