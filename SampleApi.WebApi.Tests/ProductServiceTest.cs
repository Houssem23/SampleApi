using SampleApi.WebApi.Models;
using SampleApi.WebApi.Repositories;
using SampleApi.WebApi.Services;
using Xunit;

namespace SampleApi.WebApi.Tests
{
    public class ProductServiceTest
    {
        IProductRepository<Product> _context;
        IService<Product> _service;

        public ProductServiceTest()
        {
            _context = new ProductRepositoryInMemoryDb();
            _service = new ProductService(_context);
        }

        [Fact]
        public async Task AddDuplicateProductShouldFailTest() 
        {
            //arrange
            await _service.Add(new Product() { Id = 0, ProductName = "productname1", ProductDescription = "productdesc1", Rank = 7 });
            //act & assert
            await Assert.ThrowsAsync<ArgumentException>(() =>  _service.Add(new Product() { Id = 1, ProductName = "productname1", ProductDescription = "productdesc2",Rank = 3 }));

        }
    }
}
