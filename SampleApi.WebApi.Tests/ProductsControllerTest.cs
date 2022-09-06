using SampleApi.WebApi.Controllers;
using SampleApi.WebApi.Models;
using SampleApi.WebApi.Repositories;
using SampleApi.WebApi.Services;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace SampleApi.WebApi.Tests
{
    public class ProductsControllerTest
    {
        ProductsController _controller;
        IProductRepository<Product> _context;
        IService<Product> _service;


        public ProductsControllerTest()
        {
            _context = new ProductRepositoryInMemoryDb();
            _service = new ProductService(_context);
            _controller = new ProductsController(_service);
        }
        [Fact]
        public async Task ShouldReturn200Test()
        {
            //arrange

            //act
            var result = await _controller.GetProducts();
            ObjectResult? okResult = result.Result as ObjectResult;
            //assert
            Assert.NotNull(result);
            Assert.NotNull(okResult);
            Assert.Equal(okResult!.StatusCode, StatusCodes.Status200OK);
        }
        [Fact]
        public async Task ShouldContainNoProductTestAsync()
        {
            //arrange

            //act
            var result = await _controller.GetProducts();
            //assert
            Assert.IsType<OkObjectResult>(result.Result);

            var list = result.Result as OkObjectResult;
            Assert.IsType<List<Product>>(list!.Value);

            var listProducts = list.Value as List<Product>;
            Assert.Empty(listProducts);

        }
        [Fact]
        public async Task ListTest()
        {
            //arrange
            await _service.Add(new Product() { Id = 0, ProductName = "productname1", ProductDescription = "productdesc1", Rank = 9 });
            //act
            var result = await _controller.GetProducts();
            //assert
            Assert.IsType<OkObjectResult>(result.Result);

            var list = result.Result as OkObjectResult; ;
            Assert.IsType<List<Product>>(list!.Value);

            var listProducts = list.Value as List<Product>;
            Assert.Single(listProducts);

        }
        [Theory]
        [InlineData(101, typeof(NotFoundResult))]
        [InlineData(0, typeof(OkObjectResult))]
        public async Task GetByIdTest(int id, Type expectedResultType)
        {
            //arrange
            await _service.Add(new Product() { Id = 0, ProductName = "productname1", ProductDescription = "productdesc1", Rank = 7 });

            //act
            var getProductResponse = await _controller.GetProduct(id);

            //assert
            Assert.NotNull(getProductResponse);
            Assert.NotNull(getProductResponse.Result);
            Assert.Equal(getProductResponse.Result!.ToString(), expectedResultType.ToString());
        }
        [Theory]
        [InlineData(101, typeof(NotFoundResult), 1)]
        [InlineData(0, typeof(OkObjectResult), 0)]
        public async Task DeleteTest(int id, Type expectedTypeResult, int expectedCount)
        {
            //arrange
            await _service.Add(new Product() { Id = 0, ProductName = "productname1", ProductDescription = "productdesc1", Rank = 7 });

            //act
            var deleteProductResponse = await _controller.DeleteProduct(id);

            //assert
            Assert.NotNull(deleteProductResponse);
            Assert.NotNull(deleteProductResponse.Result);
            Assert.Equal(expectedTypeResult.ToString(), deleteProductResponse.Result!.ToString());
            Assert.Equal(expectedCount, _service.List().Result!.Count());
        }
        [Fact]
        public async Task AddTest()
        {
            //arrange
            await _service.Add(new Product() { Id = 0, ProductName = "productname1", ProductDescription = "productdesc1", Rank = 7 });
            //act
            var getAddedProductResponse = await _controller.GetProduct(0);
            var result = getAddedProductResponse.Result as OkObjectResult;
            var product = result!.Value as Product;
            //assert
            Assert.NotNull(getAddedProductResponse);
            Assert.IsType<OkObjectResult>(getAddedProductResponse.Result);
            Assert.NotNull(result);
            Assert.NotNull(product);
            Assert.Equal("productname1", product!.ProductName);
        }
        [Fact]
        public async Task UpdateTest()
        {
            //arrange
            await _service.Add(new Product() { Id = 0, ProductName = "productname1", ProductDescription = "productdesc1", Rank = 7 });
            await _service.Update(new Product() { Id = 0, ProductName = "productname1updated", ProductDescription = "productdesc1updated", Rank = 3 });
            //act
            var getUpdatedProductResponse = await _controller.GetProduct(0);
            OkObjectResult? result = getUpdatedProductResponse.Result as OkObjectResult;
            Product? product = result!.Value as Product;
            //assert
            Assert.NotNull(getUpdatedProductResponse);
            Assert.IsType<OkObjectResult>(getUpdatedProductResponse.Result);
            Assert.NotNull(result);
            Assert.NotNull(product);
            Assert.Equal("productname1updated", product!.ProductName);
            Assert.Equal(3, product.Rank);
        }
    }
}
