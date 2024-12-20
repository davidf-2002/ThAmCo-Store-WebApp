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
            // Return the static list of products
            return Task.FromResult<IEnumerable<ProductDTO>>(_products);
        }
    }