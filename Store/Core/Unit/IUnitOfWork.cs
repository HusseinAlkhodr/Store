
using Store.Core.IRepository;

namespace Store.Core.Unit
{
    public interface IUnitOfWork : IDisposable
    {
        IAccountRepository AccountRepository { get; }
        IRoleRepository RoleRepository { get; }
        IUserRoleRepository UserRoleRepository { get; }
        IDivisionRepository DivisionRepository { get; }
        IItemRepository ItemRepository { get; }
        IVendorRepository VendorRepository { get; }
        IPriceListRepository PriceListRepository { get; }
        IItemTypeRepository ItemTypeRepository { get; }
        IPurchaseBillItemRepository PurchaseBillItemRepository { get; }
        IPurchaseBillRepository PurchaseBillRepository { get; }
        ISaleBillItemRepository SaleBillItemRepository { get; }
        ISaleBillRepository SaleBillRepository { get; }
        ICurrencyExchangeRateRepository CurrencyExchangeRateRepository { get; }
        Task Save();
        void SaveAsync();
    }
}
