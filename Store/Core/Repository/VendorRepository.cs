using AutoMapper;
using Store.Core.IRepository;
using Store.DataBaseContext;
using Store.Models;

namespace Store.Core.Repository
{
    public class VendorRepository : GenericRepository<Vendor>, IVendorRepository
    {
        public VendorRepository(StoreDbContext context, IMapper map) : base(context, map)
        {
        }
    }
}
