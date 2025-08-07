using AutoMapper;
using Store.Core.IRepository;
using Store.DataBaseContext;
using Store.Models;

namespace Store.Core.Repository
{
    public class PriceListRepository : GenericRepository<PriceList>, IPriceListRepository
    {
        public PriceListRepository(StoreDbContext context, IMapper map) : base(context, map)
        {
        }
    }
}
