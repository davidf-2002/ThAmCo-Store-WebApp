 public class ProductsServiceFake : IProductsService
    {
        private static readonly List<ProductDTO> _products = new List<ProductDTO>
        {
            new ProductDTO { Id = 1, Name = "Laptop", Description = "High-performance laptop", Price = 1200.00m },
            new ProductDTO { Id = 2, Name = "Smartphone", Description = "Latest model smartphone", Price = 800.00m },
            new ProductDTO { Id = 3, Name = "Tablet", Description = "Portable and powerful tablet", Price = 600.00m }
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
