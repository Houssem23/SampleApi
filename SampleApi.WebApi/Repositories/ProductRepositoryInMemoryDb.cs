using SampleApi.WebApi.Models;

namespace SampleApi.WebApi.Repositories
{
    public class ProductRepositoryInMemoryDb : IProductRepository<Product>
    {

        readonly List<Product> _products = new List<Product>();
        public async Task<Product> Add(Product product)
        {
            await Task.Run(() => _products.Add(product));
            return product;
        }
        public async Task<Product> Delete(int id)
        {
            var productToDelete = _products.First(x => x.Id == id);
            await Task.Run(() => _products.Remove(productToDelete));
            return productToDelete;
        }
        public async Task<Product> Get(int id) => await Task.Run(() => _products.Where(x => x.Id == id).FirstOrDefault());
        public async Task<IEnumerable<Product>> List(CancellationToken cancellationToken = default(CancellationToken))
        {
            List<Product> products = new List<Product>();
            try
            {
                await Task.Delay(4000);
                //Handle the task cancellation by throwing an exception
                if (cancellationToken.IsCancellationRequested) { throw new TaskCanceledException(); }
                products = _products.ToList();
                return products;
            }
            catch (TaskCanceledException ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }

        }
        public async Task<Product> Update(Product product)
        {
            var productsToUpdate = _products.Find(x => x.Id == product.Id);
            var indexOf = _products.IndexOf(productsToUpdate);
            _products[indexOf] = product;
            return product;
        }
        public async Task<Product> GetByName(string productName)
        {
            var product = await Task.Run(() => _products.Where(x => x.ProductName == productName).FirstOrDefault());
            return product;
        }
    }
}
