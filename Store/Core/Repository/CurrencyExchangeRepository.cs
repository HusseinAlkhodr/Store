using AutoMapper;
using Store.Core.IRepository;
using Store.DataBaseContext;
using Store.Models.Currency;

namespace Store.Core.Repository
{
    public class CurrencyExchangeRepository : GenericRepository<CurrencyExchangeRate>, ICurrencyExchangeRateRepository
    {
        public CurrencyExchangeRepository(StoreDbContext context, IMapper map) : base(context, map)
        {
        }
    }
}
