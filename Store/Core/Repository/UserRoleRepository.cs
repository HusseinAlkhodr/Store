using AutoMapper;
using Store.Core.IRepository;
using Store.DataBaseContext;
using Store.Models.Authenitication;

namespace Store.Core.Repository
{
    public class UserRoleRepository : GenericRepository<UserRole>, IUserRoleRepository
    {
        public UserRoleRepository(StoreDbContext context, IMapper map) : base(context, map)
        {
        }
    }
}
