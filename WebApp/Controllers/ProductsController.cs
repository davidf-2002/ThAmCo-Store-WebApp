using Microsoft.AspNetCore.Mvc;

public class ProductsController : Controller
{
    private readonly IProductsService _service;

    public ProductsController(IProductsService productsService)
    {
        _service = productsService;
    }

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

    // Implement other actions as needed
}
