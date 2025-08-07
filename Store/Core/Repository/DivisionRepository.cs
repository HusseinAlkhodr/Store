using AutoMapper;
using Store.Core.IRepository;
using Store.DataBaseContext;
using Store.Models;

namespace Store.Core.Repository
{
    public class DivisionRepository : GenericRepository<Division>, IDivisionRepository
    {
        public DivisionRepository(StoreDbContext context, IMapper map) : base(context, map)
        {
        }
    }
}
