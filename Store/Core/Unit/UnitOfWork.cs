using AutoMapper;
using Store.Core.IRepository;
using Store.Core.Repository;
using Store.DataBaseContext;

namespace Store.Core.Unit
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly StoreDbContext dbContext;
        private readonly ILoggerFactory logger;
        private readonly ILogger log;
        private readonly IMapper mapper;

        private IAccountRepository accountRepository;
        private IRoleRepository roleRepository;
        private IUserRoleRepository userRoleRepository;
        private IDivisionRepository divisionRepository;
        private IItemRepository itemRepository;
        private IVendorRepository vendorRepository;
        private IPriceListRepository priceRepository;
        private IItemTypeRepository itemTypeRepository;
        private IPurchaseBillItemRepository purchaseBillItemRepository;
        private IPurchaseBillRepository purchaseBillRepository;
        private ISaleBillItemRepository saleBillItemRepository;
        private ISaleBillRepository saleBillRepository;
        private ICurrencyExchangeRateRepository currencyExchangeRateRepository;
        public UnitOfWork(StoreDbContext dbContext, ILoggerFactory logger, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.logger = logger;
            log = logger.CreateLogger("logs");
            this.mapper = mapper;
        }
        public IAccountRepository AccountRepository =>
           accountRepository ??= new AccountRepository(dbContext, mapper);
        public IRoleRepository RoleRepository =>
            roleRepository ??= new RoleRepository(dbContext, mapper);
        public IUserRoleRepository UserRoleRepository =>
            userRoleRepository ??= new UserRoleRepository(dbContext, mapper);
        public IDivisionRepository DivisionRepository =>
            divisionRepository ??= new DivisionRepository(dbContext, mapper);
        public IItemRepository ItemRepository =>
            itemRepository ??= new ItemRepository(dbContext, mapper);
        public IVendorRepository VendorRepository =>
            vendorRepository ??= new VendorRepository(dbContext, mapper);
        public IPriceListRepository PriceListRepository =>
            priceRepository ??= new PriceListRepository(dbContext, mapper);

        public IPurchaseBillItemRepository PurchaseBillItemRepository =>
            purchaseBillItemRepository ??= new PurchaseBillItemRepository(dbContext, mapper);
        public IPurchaseBillRepository PurchaseBillRepository =>
            purchaseBillRepository ??= new PurchaseBillRepository(dbContext, mapper);

        public ISaleBillItemRepository SaleBillItemRepository =>
            saleBillItemRepository ??= new SaleBillItemRepository(dbContext, mapper);

        public ISaleBillRepository SaleBillRepository =>
            saleBillRepository ??= new SaleBillRepository(dbContext, mapper);

        public ICurrencyExchangeRateRepository CurrencyExchangeRateRepository =>
            currencyExchangeRateRepository ??= new CurrencyExchangeRepository(dbContext, mapper);

        public IItemTypeRepository ItemTypeRepository =>
            itemTypeRepository ??= new ItemTypeRepository(dbContext, mapper);

        public void Dispose()
        {
            dbContext.Dispose();
            GC.SuppressFinalize(this);
        }

        public async Task Save()
        {
            await dbContext.SaveChangesAsync();
        }

        public void SaveAsync()
        {
            dbContext.SaveChanges();
        }

    }
}
