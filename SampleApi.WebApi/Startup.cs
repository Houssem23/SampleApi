using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Extensions.NETCore.Setup;
using SampleApi.WebApi.Repositories;
using SampleApi.WebApi.Models;
using SampleApi.WebApi.Services;

namespace SampleApi.WebApi;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;

    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddAWSService<IAmazonDynamoDB>();
        services.AddDefaultAWSOptions(
            new AWSOptions
            {
                Region = RegionEndpoint.GetBySystemName("eu-west-1")
            });
        services.AddScoped<IService<Product>, ProductService>();
        services.AddSingleton<IProductRepository<Product>, ProductRepositoryDynamoDb>();
        services.AddControllers();

    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}