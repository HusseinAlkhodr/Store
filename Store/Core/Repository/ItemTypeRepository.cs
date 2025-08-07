using AutoMapper;
using Store.Core.IRepository;
using Store.DataBaseContext;
using Store.Models;

namespace Store.Core.Repository
{
    public class ItemTypeRepository : GenericRepository<ItemType>, IItemTypeRepository
    {
        public ItemTypeRepository(StoreDbContext context, IMapper map) : base(context, map)
        {
        }
    }
}
