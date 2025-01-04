using Microsoft.AspNetCore.Mvc;

public interface IProductsService
{
    Task<IEnumerable<ProductDTO>> GetProductsAsync();
    Task<ProductDTO> GetProductByIdAsync(int id);
    Task<ProductDTO> AddProductAsync(ProductDTO product);
    Task<bool> DeleteProductAsync(int id);
}
