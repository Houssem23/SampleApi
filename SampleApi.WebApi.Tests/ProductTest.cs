using SampleApi.WebApi;
using SampleApi.WebApi.Models;
using Newtonsoft.Json;
using SampleApi.WebApi.Tests.Setup;
using System.Net;
using System.Text;
using Xunit;

namespace SampleApi.WebApi.Tests
{
    [Collection("api")]
    public class ProductTest : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
    /*
        readonly HttpClient _client;

        public ProductTest(CustomWebApplicationFactory<Startup> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task AddProductReturnsOkStatus()
        {
            const int Id = 1;

            var response = await AddProductData(Id);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
        private async Task<HttpResponseMessage> AddProductData(int testId, string productName = "test-ProductName")
        {
            var productDbData = new Product
            {
                Id = testId,
                ProductName = productName,
                ProductDescription = "test-Description",
                Rank = 4
            };

            var json = JsonConvert.SerializeObject(productDbData);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

            return await _client.PostAsync($"/api/products", stringContent);
        }
    }
    */
}
