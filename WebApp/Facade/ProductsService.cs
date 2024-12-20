using System.Net.Http.Headers;

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
        var response = await _client.GetAsync("products");
        response.EnsureSuccessStatusCode();
        var products = await response.Content.ReadFromJsonAsync<IEnumerable<ProductDTO>>();
        return products;
    }

    public async Task<bool> DeleteProductAsync(int id) 
    {
        var accessToken = await GetAccessTokenAsync();

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
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
        tokenResponse.EnsureSuccessStatusCode();
        var tokenInfo = await tokenResponse.Content.ReadFromJsonAsync<TokenDto>();
        return tokenInfo.AccessToken;
    }
    
    public class TokenDto
    {
        public string AccessToken { get; set; }
    }
}
