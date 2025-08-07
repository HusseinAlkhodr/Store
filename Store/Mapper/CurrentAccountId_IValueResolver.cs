using AutoMapper;
using Store.Extensions;

namespace Store.Mapper
{
    public class CurrentAccountId_IValueResolver : IValueResolver<object, object, long?>
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public CurrentAccountId_IValueResolver(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }
        public long? Resolve(object source, object destination, long? destMember, ResolutionContext context)
        {
            return httpContextAccessor.HttpContext.User.GetCurrentAccountId();
        }
    }
}