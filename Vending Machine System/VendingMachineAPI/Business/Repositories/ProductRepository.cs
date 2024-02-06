using Microsoft.EntityFrameworkCore;
using VendingMachineAPI.Data;
using VendingMachineAPI.Models;

namespace VendingMachineAPI.Business.Repositories
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        public ProductRepository(MachineDBContext context, ILogger logger) 
            : base(context, logger)
        {
        }

        public new async Task<IEnumerable<object>> AllAsync()
        {
            var products = await _context.Products.Select(p => new
            {
                p.ProductId,
                p.ProductName,
                p.Cost,
                p.AmountAvailabe,
                SellerName = p.Seller.UserName
            }).ToListAsync();

            return products;
        }
        public async Task<Product?> GetByName(string Name)
        {
            return await _context.Products.FirstOrDefaultAsync(p => p.ProductName == Name);
        }
    }
}
