 public class ProductsServiceFake : IProductsService
    {
    private readonly List<ProductDTO> _products = new List<ProductDTO>
    {
        new ProductDTO { Id = 1, Name = "T-shirt", Description = "Jack & Jones", Price = 11.50m, StockStatus = "In Stock", LastUpdated = new DateTime(2024, 11, 07)},
        new ProductDTO { Id = 2, Name = "Jeans", Description = "Armani", Price = 30.00m, StockStatus = "In Stock", LastUpdated = new DateTime(2024, 11, 07)},
        new ProductDTO { Id = 3, Name = "Hoody", Description = "Boss", Price = 20.99m, StockStatus = "Out of Stock", LastUpdated = new DateTime(2024, 11, 07)}    
    };

        public Task<IEnumerable<ProductDTO>> GetProductsAsync()
        {
            return Task.FromResult<IEnumerable<ProductDTO>>(_products);
        }

        public Task<ProductDTO> GetProductByIdAsync(int id)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            return Task.FromResult(product);
        }

        public async Task<ProductDTO> AddProductAsync(ProductDTO product)
        {
            _products.Add(product);
            return await Task.FromResult(product);
        }

        public Task<bool> DeleteProductAsync(int id)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                return Task.FromResult(false);
            }

            _products.Remove(product);
            return Task.FromResult(true);
        }
    }
