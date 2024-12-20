public class ProductsService : IProductsService
{
    private readonly HttpClient _client;

    public ProductsService(IHttpClientFactory clientFactory)
    {
        _client = clientFactory.CreateClient("ProductsClient");
    }

    public async Task<IEnumerable<ProductDTO>> GetProductsAsync()
    {
        var response = await _client.GetAsync("products");
        response.EnsureSuccessStatusCode();
        var products = await response.Content.ReadFromJsonAsync<IEnumerable<ProductDTO>>();
        return products;
    }
}
