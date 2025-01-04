using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApp.Models;
using WebApp.Controllers; 

namespace WebApp.Tests
{
    [TestClass]
    public class ProductsControllerTests
    {
        private Mock<IProductsService> _mockService;
        private ProductsController _controller;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockService = new Mock<IProductsService>();
            _controller = new ProductsController(_mockService.Object);
        }

        /// <summary>
        /// Ensures the Index action returns a list of products in the View.
        /// </summary>
        [TestMethod]
        public async Task Index_ReturnsViewResultWithListOfProductViewModels()
        {
            // Arrange
            var fakeProducts = new List<ProductDTO>
            {
                new ProductDTO { Id = 1, Name = "Product A", Description = "Desc A", Price = 10.0M },
                new ProductDTO { Id = 2, Name = "Product B", Description = "Desc B", Price = 20.0M },
            };
            _mockService.Setup(s => s.GetProductsAsync()).ReturnsAsync(fakeProducts);

            // Act
            var result = await _controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Model, typeof(IEnumerable<ProductViewModel>));
            var model = result.Model as IEnumerable<ProductViewModel>;
            Assert.AreEqual(2, model.Count());
        }

        /// <summary>
        /// Verifies Details returns NotFound when the ID is not valid or null.
        /// </summary>
        [TestMethod]
        public async Task Details_InvalidId_ReturnsNotFound()
        {
            // Act
            var result = await _controller.Details(0); // Using 0 or negative for invalid

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        /// <summary>
        /// Verifies Details returns NotFound when the product does not exist.
        /// </summary>
        [TestMethod]
        public async Task Details_ProductNotFound_ReturnsNotFound()
        {
            // Arrange
            _mockService.Setup(s => s.GetProductByIdAsync(It.IsAny<int>())).ReturnsAsync((ProductDTO)null);

            // Act
            var result = await _controller.Details(123);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        /// <summary>
        /// Verifies Details returns a valid product view model when the product is found.
        /// </summary>
        [TestMethod]
        public async Task Details_ValidId_ReturnsViewResultWithProduct()
        {
            // Arrange
            var productDto = new ProductDTO
            {
                Id = 1,
                Name = "Test",
                Description = "Test Desc",
                Price = 100m
            };
            _mockService.Setup(s => s.GetProductByIdAsync(1)).ReturnsAsync(productDto);

            // Act
            var result = await _controller.Details(1) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Model, typeof(ProductViewModel));
            var model = result.Model as ProductViewModel;
            Assert.AreEqual(productDto.Id, model.Id);
            Assert.AreEqual(productDto.Name, model.Name);
        }

        /// <summary>
        /// Verifies the Create (GET) method simply returns a View.
        /// </summary>
        [TestMethod]
        public void Create_Get_ReturnsView()
        {
            // Act
            var result = _controller.Create() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

        /// <summary>
        /// Verifies that Create (POST) returns the same View when ModelState is invalid.
        /// </summary>
        [TestMethod]
        public async Task Create_Post_InvalidModel_ReturnsView()
        {
            // Arrange
            _controller.ModelState.AddModelError("Name", "Required");
            var viewModel = new ProductViewModel(); // Missing required fields

            // Act
            var result = await _controller.Create(viewModel) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(viewModel, result.Model);
        }

        /// <summary>
        /// Verifies that Create (POST) redirects to Index on success.
        /// </summary>
        [TestMethod]
        public async Task Create_Post_ValidModel_RedirectsToIndex()
        {
            // Arrange
            var viewModel = new ProductViewModel
            {
                Name = "Valid Product",
                Description = "Valid Desc",
                Price = 99.99M,
                StockLevel = 10,
                CategoryId = 2
            };

            // Act
            var result = await _controller.Create(viewModel) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ActionName);
            _mockService.Verify(s => s.AddProductAsync(It.IsAny<ProductDTO>()), Times.Once);
        }

        /// <summary>
        /// Verifies Delete (GET) returns NotFound if the product is not found.
        /// </summary>
        [TestMethod]
        public async Task Delete_Get_ProductNotFound_ReturnsNotFound()
        {
            // Arrange
            _mockService.Setup(s => s.GetProductByIdAsync(It.IsAny<int>())).ReturnsAsync((ProductDTO)null);

            // Act
            var result = await _controller.Delete(999);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        /// <summary>
        /// Verifies Delete (GET) returns the View with the correct product.
        /// </summary>
        [TestMethod]
        public async Task Delete_Get_ValidId_ReturnsViewResult()
        {
            // Arrange
            var productDto = new ProductDTO { Id = 1, Name = "Test", Description = "Test Desc", Price = 50.0M };
            _mockService.Setup(s => s.GetProductByIdAsync(1)).ReturnsAsync(productDto);

            // Act
            var result = await _controller.Delete(1) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Model, typeof(ProductViewModel));
            var model = result.Model as ProductViewModel;
            Assert.AreEqual(productDto.Id, model.Id);
        }

        /// <summary>
        /// Verifies DeleteConfirmed calls the service and redirects to Index.
        /// </summary>
        [TestMethod]
        public async Task DeleteConfirmed_ValidId_RedirectsToIndex()
        {
            // Arrange
            var idToDelete = 1;

            // Act
            var result = await _controller.DeleteConfirmed(idToDelete) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ActionName);
            _mockService.Verify(s => s.DeleteProductAsync(idToDelete), Times.Once);
        }
    }
}
