using SampleApi.WebApi.Models;
using SampleApi.WebApi.Repositories;

namespace SampleApi.WebApi.Services
{
    public class ProductService : IService<Product>
    {
        readonly IProductRepository<Product> _context;
        public ProductService(IProductRepository<Product> context)
        {
            _context = context;
        }
        public async Task<bool> DoesItemExistAsync(Product item)
        {
            var productByName = await _context.GetByName(item.ProductName!);
            var productById = await _context.Get(item.Id);
            return productByName != null || productById != null;
        }

        public async Task<Product> Add(Product item)
        {
            if (!await DoesItemExistAsync(item))
            {
                return await _context.Add(item);
            }
            else
            {

                throw new ArgumentException();

            }
        }

        public async Task<Product> Delete(int id)
        {
            return await _context.Delete(id);
        }

        public async Task<Product?> GetById(int id)
        {
            return await _context.Get(id);
        }

        public async Task<IEnumerable<Product>?> List(CancellationToken cancellationToken = default(CancellationToken))
        {
            return await _context.List(cancellationToken);
        }

        public async Task<Product> Update(Product item)
        {
            return await _context.Update(item);
        }
    }
}
