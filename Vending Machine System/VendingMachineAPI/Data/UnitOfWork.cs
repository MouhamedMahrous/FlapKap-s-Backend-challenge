using VendingMachineAPI.Business;
using VendingMachineAPI.Business.Repositories;

namespace VendingMachineAPI.Data
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly MachineDBContext _context;
        public IProductRepository Products {  get; private set; }

        public UnitOfWork(MachineDBContext context, ILoggerFactory logger)
        {
            _context = context;
            var _logger = logger.CreateLogger("logs");
            Products = new ProductRepository(context, _logger);
        }

        public async Task CompleteAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
