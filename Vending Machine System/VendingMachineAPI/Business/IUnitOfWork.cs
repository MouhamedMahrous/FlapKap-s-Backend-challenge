namespace VendingMachineAPI.Business
{
    public interface IUnitOfWork
    {
        IProductRepository Products { get; }
        Task CompleteAsync();
    }
}
