using AutoMapper;
using Store.Core.IRepository;
using Store.DataBaseContext;
using Store.Models.Authenitication;

namespace Store.Core.Repository
{
    public class RoleRepository : GenericRepository<Role>, IRoleRepository
    {
        public RoleRepository(StoreDbContext context, IMapper map) : base(context, map)
        {
        }
    }
}
