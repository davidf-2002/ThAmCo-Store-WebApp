public interface IProductsService
{
    Task<IEnumerable<ProductDTO>> GetProductsAsync();
    //Task<ProductDTO> GetProductAsync(int Id);
    
}