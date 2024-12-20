public class ProductsService : IProductsService
{
    private readonly HttpClient _client;
    private readonly IConfiguration _config;

    public ProductsService(HttpClient client, IConfiguration configuration)
    {
        _client = client;
        _config = configuration;
    }

    public async Task<IEnumerable<ProductDTO>> GetProductsAsync()
    {
        var response = await _client.GetAsync("products");
        response.EnsureSuccessStatusCode();
        var products = await response.Content.ReadFromJsonAsync<IEnumerable<ProductDTO>>();
        return products;
    }
}
