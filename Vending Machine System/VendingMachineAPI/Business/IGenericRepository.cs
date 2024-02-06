namespace VendingMachineAPI.Business
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IEnumerable<T>> AllAsync();
        Task<T?> GetByIdAsync(int id);
        Task<bool> AddAsync(T entity);
        bool Update(T entity);
        bool Delete(T entity);
    }
}
