using VendingMachineAPI.Models;

namespace VendingMachineAPI.Business
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        new Task<IEnumerable<object>> AllAsync();
        Task<Product?> GetByName(string Name);
    }
}
