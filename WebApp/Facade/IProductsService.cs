public interface IProductsService
{
    Task<IEnumerable<ProductDTO>> GetProductsAsync();
    Task<bool> DeleteProductAsync(int id);
}