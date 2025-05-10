namespace CELERATE.API.CORE.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task<T> GetByIdAsync(string id);
        Task<IReadOnlyList<T>> GetAllAsync();
        Task<string> AddAsync(T entity);
        Task UpdateAsync(T entity);
    }
}
