using Moq;
using Moq.Protected;
using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace WebApp.Tests
{
    [TestClass]
    public class ProductsServiceTests
    {
        private Mock<IHttpContextAccessor> _mockHttpContextAccessor;
        private Mock<IHttpClientFactory> _mockHttpClientFactory;
        private ProductsService _service;

        [TestInitialize]
        public void Setup()
        {
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            _mockHttpClientFactory = new Mock<IHttpClientFactory>();

            var identity = new ClaimsIdentity(new[] { new Claim("typ", "JWT") }, "TestAuthType");
            var principal = new ClaimsPrincipal(identity);
            var context = new DefaultHttpContext
            {
                User = principal
            };

            var mockAuthService = new Mock<IAuthenticationService>();
            mockAuthService
                .Setup(x => x.AuthenticateAsync(It.IsAny<HttpContext>(), It.IsAny<string>()))
                .ReturnsAsync(AuthenticateResult.Success(
                    new AuthenticationTicket(principal, "TestAuthType")));

            var services = new ServiceCollection();
            services.AddSingleton<IAuthenticationService>(mockAuthService.Object);
            var serviceProvider = services.BuildServiceProvider();
            context.RequestServices = serviceProvider;

            _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(context);

            _service = new ProductsService(_mockHttpContextAccessor.Object, _mockHttpClientFactory.Object);
        }

        /// <summary>
        /// Utility helper to create a mock HttpMessageHandler that responds with the specified status code and content.
        /// </summary>
        private static Mock<HttpMessageHandler> CreateMockMessageHandler(HttpStatusCode statusCode, string content)
        {
            var mockHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            mockHandler
               .Protected()
               .Setup<Task<HttpResponseMessage>>(
                   "SendAsync",
                   ItExpr.IsAny<HttpRequestMessage>(),
                   ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync((HttpRequestMessage request, CancellationToken token) =>
               {
                   var response = new HttpResponseMessage
                   {
                       StatusCode = statusCode,
                       Content = new StringContent(content, Encoding.UTF8, "application/json")
                   };
                   return response;
               });
            return mockHandler;
        }

        /// <summary>
        /// Tests that GetProductsAsync returns a valid list of products.
        /// </summary>
        [TestMethod]
        public async Task GetProductsAsync_ReturnsListOfProducts()
        {
            // Arrange
            var fakeProducts = new List<ProductDTO>
            {
                new ProductDTO { Id = 1, Name = "Product A", Description = "Desc A", Price = 10.0M },
                new ProductDTO { Id = 2, Name = "Product B", Description = "Desc B", Price = 20.0M }
            };
            var jsonContent = JsonSerializer.Serialize(fakeProducts);
            var mockHandler = CreateMockMessageHandler(HttpStatusCode.OK, jsonContent);

            // Set BaseAddress to avoid the "invalid request URI" error
            var httpClient = new HttpClient(mockHandler.Object)
            {
                BaseAddress = new Uri("http://localhost/")
            };
            _mockHttpClientFactory
                .Setup(f => f.CreateClient("ProductsClient"))
                .Returns(httpClient);

            // Act
            var result = await _service.GetProductsAsync();

            // Assert
            Assert.IsNotNull(result);
            var resultList = new List<ProductDTO>(result);
            Assert.AreEqual(2, resultList.Count);
            Assert.AreEqual("Product A", resultList[0].Name);
        }

        /// <summary>
        /// Tests that GetProductByIdAsync returns a single product.
        /// </summary>
        [TestMethod]
        public async Task GetProductByIdAsync_ReturnsProduct()
        {
            // Arrange
            var fakeProduct = new ProductDTO
            {
                Id = 1,
                Name = "Test Product",
                Description = "Test Description",
                Price = 50.5M
            };
            var jsonContent = JsonSerializer.Serialize(fakeProduct);
            var mockHandler = CreateMockMessageHandler(HttpStatusCode.OK, jsonContent);

            var httpClient = new HttpClient(mockHandler.Object)
            {
                BaseAddress = new Uri("http://localhost/")
            };            
            _mockHttpClientFactory
                .Setup(f => f.CreateClient("ProductsClient"))
                .Returns(httpClient);

            // Act
            var result = await _service.GetProductByIdAsync(1);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(fakeProduct.Id, result.Id);
            Assert.AreEqual(fakeProduct.Name, result.Name);
        }

        /// <summary>
        /// Tests that AddProductAsync sends the product and returns the created product.
        /// </summary>
        [TestMethod]
        public async Task AddProductAsync_CreatesProduct_ReturnsCreatedProduct()
        {
            // Arrange
            var productToCreate = new ProductDTO
            {
                Name = "New Product",
                Description = "New Desc",
                Price = 100.0M
            };
            var createdProduct = new ProductDTO
            {
                Id = 100,
                Name = "New Product",
                Description = "New Desc",
                Price = 100.0M
            };
            var jsonContent = JsonSerializer.Serialize(createdProduct);
            var mockHandler = CreateMockMessageHandler(HttpStatusCode.OK, jsonContent);

            var httpClient = new HttpClient(mockHandler.Object)
            {
                BaseAddress = new Uri("http://localhost/")
            };            
            _mockHttpClientFactory
                .Setup(f => f.CreateClient("ProductsClient"))
                .Returns(httpClient);

            // Act
            var result = await _service.AddProductAsync(productToCreate);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(createdProduct.Id, result.Id);
            Assert.AreEqual(createdProduct.Name, result.Name);

            // Optional: verify the Authorization header was set
            Assert.AreEqual("Bearer", httpClient.DefaultRequestHeaders.Authorization.Scheme);
        }

        /// <summary>
        /// Tests that DeleteProductAsync returns true when response status is success and the response content includes "has been deleted".
        /// </summary>
        [TestMethod]
        public async Task DeleteProductAsync_Successful_ReturnsTrue()
        {
            // Arrange
            var successContent = "Product has been deleted"; 
            var mockHandler = CreateMockMessageHandler(HttpStatusCode.OK, successContent);
            var httpClient = new HttpClient(mockHandler.Object)
            {
                BaseAddress = new Uri("http://localhost/")
            };            
            _mockHttpClientFactory
                .Setup(f => f.CreateClient("ProductsClient"))
                .Returns(httpClient);

            // Act
            var result = await _service.DeleteProductAsync(1);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual("Bearer", httpClient.DefaultRequestHeaders.Authorization.Scheme);
        }

        /// <summary>
        /// Tests that DeleteProductAsync returns false when response status is non-success.
        /// </summary>
        [TestMethod]
        public async Task DeleteProductAsync_Unsuccessful_ReturnsFalse()
        {
            // Arrange
            var mockHandler = CreateMockMessageHandler(HttpStatusCode.BadRequest, "Some error");
            var httpClient = new HttpClient(mockHandler.Object)
            {
                BaseAddress = new Uri("http://localhost/")
            };            
            _mockHttpClientFactory
                .Setup(f => f.CreateClient("ProductsClient"))
                .Returns(httpClient);

            // Act
            var result = await _service.DeleteProductAsync(1);

            // Assert
            Assert.IsFalse(result);
        }

        /// <summary>
        /// Tests that GetProductByIdAsync throws an exception if status code is not successful.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(HttpRequestException))]
        public async Task GetProductByIdAsync_NonSuccessStatus_ThrowsHttpRequestException()
        {
            // Arrange
            var mockHandler = CreateMockMessageHandler(HttpStatusCode.NotFound, "Not Found");
            var httpClient = new HttpClient(mockHandler.Object)
            {
                BaseAddress = new Uri("http://localhost/")
            };            
            _mockHttpClientFactory
                .Setup(f => f.CreateClient("ProductsClient"))
                .Returns(httpClient);

            // Act
            // Should throw HttpRequestException due to response.EnsureSuccessStatusCode()
            await _service.GetProductByIdAsync(999);
        }
    }
}
