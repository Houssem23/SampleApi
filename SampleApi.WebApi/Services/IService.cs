using System.Collections.Generic;

namespace SampleApi.WebApi.Services
{
    public interface IService<T>
    {
        Task<IEnumerable<T>?> List(CancellationToken cancellationToken = default(CancellationToken));
        Task<T?> GetById(int id);
        Task<T> Add(T item);
        Task<T> Update(T item);
        Task<T> Delete(int id);

    }
}
