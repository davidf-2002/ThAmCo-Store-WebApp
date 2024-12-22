using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;

public class ProductsService : IProductsService
{
    private readonly IConfiguration _configuration;
    private readonly IHttpClientFactory _clientFactory;


    public ProductsService(IHttpClientFactory clientFactory, IConfiguration configuration)
    {
        _clientFactory = clientFactory;
        _configuration = configuration;
    }

    public async Task<IEnumerable<ProductDTO>> GetProductsAsync()
    {
        // Retrieve the access token
        var accessToken = await GetAccessTokenAsync(); 
        if (accessToken == null)
        {
            throw new InvalidOperationException("Failed to retrieve access token.");
        }
        var apiClient = _clientFactory.CreateClient("ProductsClient");
        apiClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken); // Set the Authorization header with the retrieved token
        Console.WriteLine("accessToken: " +  accessToken);

        var response = await apiClient.GetAsync("products");
        response.EnsureSuccessStatusCode(); // This will throw an exception if the HTTP response status is an error code

        // Read the JSON response and convert it to IEnumerable<ProductDTO>
        var products = await response.Content.ReadFromJsonAsync<IEnumerable<ProductDTO>>();
        
        return products;
    }

    public async Task<ProductDTO> GetProductByIdAsync(int id)
    {
        var _client = _clientFactory.CreateClient("ProductsClient");
        var response = await _client.GetAsync($"products/{id}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ProductDTO>();
    }

    public async Task<bool> DeleteProductAsync(int id) 
    {
        var accessToken = await GetAccessTokenAsync();
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

    private async Task<string> GetAccessTokenAsync()
    {
        var tokenClient = _clientFactory.CreateClient();
        var tokenParams = new Dictionary<string, string>
        {
            { "grant_type", "client_credentials" },
            { "client_id", _configuration["Auth0:ClientId"] },
            { "client_secret", _configuration["Auth:ClientSecret"] },
            { "audience", _configuration["Auth0:Audience"] }
        };

        var tokenForm = new FormUrlEncodedContent(tokenParams);
        tokenClient.BaseAddress = new Uri(_configuration["Auth:Authority"]);

        var tokenResponse = await tokenClient.PostAsync("oauth/token", tokenForm);
        var responseBody = await tokenResponse.Content.ReadAsStringAsync();
        Console.WriteLine($"Token Response: {responseBody}");

        if (!tokenResponse.IsSuccessStatusCode)
        {
            Console.WriteLine("Token request failed.");
            return null; 
        }

        var tokenInfo = JsonSerializer.Deserialize<TokenDto>(responseBody);
        return tokenInfo?.AccessToken;
    }
    
    private class TokenDto
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonPropertyName("token_type")]
        public string TokenType { get; set; }

        [JsonPropertyName("scope")]
        public string? Scope { get; set; }
    }
}