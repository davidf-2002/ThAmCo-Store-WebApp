using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;

public class ProductsService : IProductsService
{
    private readonly HttpClient _client;
    private readonly IConfiguration _configuration;
    private readonly IHttpClientFactory _clientFactory;


    public ProductsService(IHttpClientFactory clientFactory, IConfiguration configuration)
    {
        _client = clientFactory.CreateClient("ProductsClient");
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
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken); // Set the Authorization header with the retrieved token

        var response = await _client.GetAsync("products");
        response.EnsureSuccessStatusCode(); // This will throw an exception if the HTTP response status is an error code

        // Read the JSON response and convert it to IEnumerable<ProductDTO>
        var products = await response.Content.ReadFromJsonAsync<IEnumerable<ProductDTO>>();
        return products;
    }


    public async Task<bool> DeleteProductAsync(int id) 
    {
        var accessToken = await GetAccessTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        Console.WriteLine("Authorization header set with token: " + _client.DefaultRequestHeaders.Authorization?.Parameter);

        var response = await _client.DeleteAsync($"products/{id}");
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
            { "client_id", _configuration["Auth:ClientId"] },
            { "client_secret", _configuration["Auth:ClientSecret"] },
            { "audience", _configuration["Auth:Audience"] }
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
    }
}