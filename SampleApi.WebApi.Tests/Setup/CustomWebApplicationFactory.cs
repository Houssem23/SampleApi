using Amazon.DynamoDBv2;
using Amazon.Runtime;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;

namespace SampleApi.WebApi.Tests.Setup
{
    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
                services.AddSingleton<IAmazonDynamoDB>(cc =>
                {
                    var clientConfig = new AmazonDynamoDBConfig { UseHttp=true, RegionEndpoint = Amazon.RegionEndpoint.EUWest1, ServiceURL = "http://localhost:8000/" };
                    return new AmazonDynamoDBClient(clientConfig);
                }));
        }
    }
}
