using SampleApi.WebApi.Models;

namespace SampleApi.WebApi.Repositories
{
    public class ProductRepositoryInMemoryDb : IProductRepository<Product>
    {

        readonly List<Product> _products = new();
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
        public async Task<Product?> Get(int id) => await Task.Run(() => _products.Where(x => x.Id == id).FirstOrDefault());
        public async Task<IEnumerable<Product>?> List(CancellationToken cancellationToken = default(CancellationToken))
        {
            List<Product> products = new();
            try
            {
                await Task.Delay(1000, cancellationToken);
                //Handle the task cancellation by throwing an exception
                if (cancellationToken.IsCancellationRequested) { throw new TaskCanceledException(); }
                products = _products.ToList();
            }
            catch (TaskCanceledException ex)
            {
                Console.WriteLine(ex.Message);
            }
            return products;
        }
        public async Task<Product> Update(Product product)
        {
            var productsToUpdate = await Task.Run(() => _products.Find(x => x.Id == product.Id));
            if (productsToUpdate != null)
            {
                var indexOf = _products.IndexOf(productsToUpdate);
                _products[indexOf] = product;
            }
            return product;
        }
        public async Task<Product?> GetByName(string productName)
        {
            var product = await Task.Run(() => _products.Where(x => x.ProductName == productName).FirstOrDefault());
            return product;
        }
    }
}
