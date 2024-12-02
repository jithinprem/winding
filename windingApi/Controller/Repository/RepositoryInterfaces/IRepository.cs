using System.Collections.Generic;
using System.Threading.Tasks;

namespace windingApi.Controller.Repository.RepositoryInterfaces;

public interface IRepository<T, TKey> where T: class
{
    public Task<IEnumerable<T>> GetAllAsync();
    public Task<T> GetByIdAsync(TKey id);
    public Task<T> AddAsync(T entity);
    public Task UpdateAsync(T entity);
    public Task DeleteAsync(TKey id);

    public Task<IEnumerable<T>> AddList(IEnumerable<T> entities);
}