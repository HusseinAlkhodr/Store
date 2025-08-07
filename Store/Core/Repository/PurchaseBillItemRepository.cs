using AutoMapper;
using Store.Core.IRepository;
using Store.DataBaseContext;
using Store.Models.Invoice;

namespace Store.Core.Repository
{
    public class PurchaseBillItemRepository : GenericRepository<PurchaseBillItem>, IPurchaseBillItemRepository
    {
        public PurchaseBillItemRepository(StoreDbContext context, IMapper map) : base(context, map)
        {
        }
    }
}
