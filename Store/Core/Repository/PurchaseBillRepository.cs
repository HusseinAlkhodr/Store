using AutoMapper;
using Store.Core.IRepository;
using Store.DataBaseContext;
using Store.Models.Invoice;

namespace Store.Core.Repository
{
    public class PurchaseBillRepository : GenericRepository<PurchaseBill>, IPurchaseBillRepository
    {
        public PurchaseBillRepository(StoreDbContext context, IMapper map) : base(context, map)
        {
        }
    }
}
