using SampleApi.WebApi.Models;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;

namespace SampleApi.WebApi.Repositories
{
    public class ProductRepositoryDynamoDb : IProductRepository<Product>
    {
        private readonly DynamoDBContext _context;
        public ProductRepositoryDynamoDb(IAmazonDynamoDB dynamoDbClient)
        {
            /*var config = new DynamoDBContextConfig
            {
                TableNamePrefix = ConfigurationSettingsConstants.CreatedRessourceTableName
            };*/
            _context = new DynamoDBContext(dynamoDbClient);
           
        }
        public async Task<Product> Add(Product item)
        {
            await _context.SaveAsync(item);
            return item;
        }

        public async Task<Product> Delete(int id)
        {
            await _context.DeleteAsync<Product>(id);
            return new Product();
        }

        public async Task<Product> Get(int id)
        {

            return await _context.LoadAsync<Product>(id);
        }
        //TODO: find a better way for this
        public async Task<Product> GetByName(string itemName)
        {
            var config = new DynamoDBOperationConfig
            {
                IndexName = "ProductName-index"
            };
            var result = _context.QueryAsync<Product>(itemName, config).GetRemainingAsync().Result;
            if (result.Any())
            {
                return result.First();
            }
            else 
            {
                return null;
            }
              
        }

        public async Task<IEnumerable<Product>> List(CancellationToken cancellationToken = default)
        {
            return await _context.ScanAsync<Product>(new List<ScanCondition>()).GetRemainingAsync();
        }

        public async Task<Product> Update(Product item)
        {
            await _context.SaveAsync(item);
            return item;
        }
    }
}
