using Microsoft.AspNetCore.Identity;

namespace Store.Models.Authenitication
{
    public class UserRole : IdentityUserRole<long>
    {
        public virtual Account User { get; set; }
        public virtual Role Role { get; set; }
    }
}
