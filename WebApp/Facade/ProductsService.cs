using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication;
using System.Text;

public class ProductsService : IProductsService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IHttpClientFactory _clientFactory;

    public ProductsService(IHttpContextAccessor httpContextAccessor, IHttpClientFactory clientFactory)
    {
        _httpContextAccessor = httpContextAccessor;
        _clientFactory = clientFactory;
    }

    public async Task<IEnumerable<ProductDTO>> GetProductsAsync()
    {
        var apiClient = _clientFactory.CreateClient("ProductsClient");
        var response = await apiClient.GetAsync("products");
        var products = await response.Content.ReadFromJsonAsync<IEnumerable<ProductDTO>>(); // Read the JSON response and convert it to IEnumerable<ProductDTO>
        
        return products;
    }

    public async Task<ProductDTO> GetProductByIdAsync(int id)
    {
        var _client = _clientFactory.CreateClient("ProductsClient");
        var response = await _client.GetAsync($"products/{id}");
        response.EnsureSuccessStatusCode();
        var product = await response.Content.ReadFromJsonAsync<ProductDTO>();

        return product;
    }

    public async Task<ProductDTO> AddProductAsync(ProductDTO product)
    {
        try
        {
            var accessToken = await _httpContextAccessor.HttpContext.GetTokenAsync("access_token");
            var _client = _clientFactory.CreateClient("ProductsClient");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            Console.WriteLine("Authorization header set with token: " + _client.DefaultRequestHeaders.Authorization?.Parameter);
            var content = JsonSerializer.Serialize(product);
            var httpResponse = await _client.PostAsync("products", new StringContent(content, Encoding.UTF8, "application/json"));

            httpResponse.EnsureSuccessStatusCode(); // Throws an exception if the HTTP response status is an error code.
            var createdProduct = await httpResponse.Content.ReadFromJsonAsync<ProductDTO>();
            return createdProduct;
        }
            catch (Exception ex)
            {
                Console.WriteLine("Error in AddProductAsync: " + ex.Message);
                throw; 
            }
    }

    public async Task<bool> DeleteProductAsync(int id) 
    {
        var accessToken = await _httpContextAccessor.HttpContext.GetTokenAsync("access_token");
        var _client = _clientFactory.CreateClient("ProductsClient");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        Console.WriteLine("Authorization header set with token: " + _client.DefaultRequestHeaders.Authorization?.Parameter);
        var response = await _client.DeleteAsync($"products/{id}");
        Console.WriteLine($"Delete response status: {response.StatusCode}");
        
        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            return responseContent.Contains("has been deleted");
        }
        return false;
    }
}