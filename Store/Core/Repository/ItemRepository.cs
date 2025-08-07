using AutoMapper;
using Store.Core.IRepository;
using Store.DataBaseContext;
using Store.Models;

namespace Store.Core.Repository
{
    public class ItemRepository : GenericRepository<Item>, IItemRepository
    {
        public ItemRepository(StoreDbContext context, IMapper map) : base(context, map)
        {
        }
    }
}
