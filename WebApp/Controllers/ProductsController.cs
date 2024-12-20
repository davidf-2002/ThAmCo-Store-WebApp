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

    [HttpDelete]
    [Route("products/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        bool isDeleted = await _service.DeleteProductAsync(id);
        if (isDeleted)
        {
            return RedirectToAction("Index"); 
        }
        else
        {
            return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
