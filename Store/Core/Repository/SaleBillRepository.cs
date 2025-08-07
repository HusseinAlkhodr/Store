using AutoMapper;
using Store.Core.IRepository;
using Store.DataBaseContext;
using Store.Models.Invoice;

namespace Store.Core.Repository
{
    public class SaleBillRepository : GenericRepository<SaleBill>, ISaleBillRepository
    {
        public SaleBillRepository(StoreDbContext context, IMapper map) : base(context, map)
        {
        }
    }
}
