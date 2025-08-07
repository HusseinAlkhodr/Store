using AutoMapper;
using Store.Core.IRepository;
using Store.DataBaseContext;
using Store.Models.Invoice;

namespace Store.Core.Repository
{
    public class SaleBillItemRepository : GenericRepository<SaleBillItem>, ISaleBillItemRepository
    {
        public SaleBillItemRepository(StoreDbContext context, IMapper map) : base(context, map)
        {
        }
    }
}
