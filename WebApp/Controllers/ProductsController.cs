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
            Price = dto.Price,
            StockStatus = dto.StockStatus
        });

        return View(productViewModels);
    }
    
    // GET: Products/Details/5
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        if (id == 0)
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
                Price = product.Price,
                StockStatus = product.StockStatus,
                StockLevel = product.StockLevel
            };
            return View(viewModel);
        }
        catch (HttpRequestException)
        {
            return View("Product Not Found");
        }
    }

    // GET: Products/Search
    [HttpGet]
    public async Task<IActionResult> Search(string query)
    {
        if (string.IsNullOrEmpty(query))
        {
            return BadRequest("Search query is required");
        }

        var products = await _service.GetProductsAsync();
        var matchingProducts = products.Where(p => p.Name.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                                                p.Description.Contains(query, StringComparison.OrdinalIgnoreCase));

        if (!matchingProducts.Any())
        {
            return View("ProductNotFound");
        }

        var productViewModels = matchingProducts.Select(p => new ProductViewModel
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Price = p.Price,
            StockStatus = p.StockStatus
        });

        return View("Index", productViewModels);
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
                StockLevel = productViewModel.StockLevel,
                CategoryId = productViewModel.CategoryId
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
