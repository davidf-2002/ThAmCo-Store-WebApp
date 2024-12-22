public interface IProductsService
{
    Task<IEnumerable<ProductDTO>> GetProductsAsync();
    Task<ProductDTO> GetProductByIdAsync(int id);
    Task<bool> DeleteProductAsync(int id);
}
