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
        var products = await _service.GetProductsAsync();
        var productViewModels = products.Select(dto => new ProductViewModel
        {
            Id = dto.Id,
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price
        });

        return View(productViewModels);
    }
    
    // GET: Products/Details/5
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        if (id == null)
        {
            return NotFound();
        }
        try
        {
            var product = await _service.GetProductByIdAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            var viewModel = new ProductViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price
            };
            return View(viewModel);
        }
        catch (HttpRequestException)
        {
            return BadRequest("Product doesn't exist");
        }
    }

    // GET: Products/Create
    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    // POST: Products/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProductViewModel productViewModel)
    {
        if (ModelState.IsValid)
        {
            ProductDTO productDTO = new ProductDTO
            {
                Name = productViewModel.Name,
                Description = productViewModel.Description,
                Price = productViewModel.Price,
                StockStatus = productViewModel.StockStatus,
            };

            await _service.AddProductAsync(productDTO);
            return RedirectToAction(nameof(Index));
        }
        return View(productViewModel);
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
