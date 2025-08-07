using AutoMapper;
using Store.Core.IRepository;
using Store.DataBaseContext;
using Store.Models.Authenitication;

namespace Store.Core.Repository
{
    public class AccountRepository : GenericRepository<Account>, IAccountRepository
    {
        public AccountRepository(StoreDbContext context, IMapper map) : base(context, map)
        {
        }
    }
}
