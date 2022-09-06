using SampleApi.WebApi.Models;

namespace SampleApi.WebApi.Repositories
{
    public interface IProductRepository<T>
    {
        Task<IEnumerable<T>?> List(CancellationToken cancellationToken = default(CancellationToken));
        Task<T?> Get(int id);
        Task<T> Update(T item);
        Task<T> Delete(int id);
        Task<T?> GetByName(string itemName);
        Task<T> Add(T item);
    }
}
