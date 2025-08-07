using Microsoft.AspNetCore.Identity;

namespace Store.Models.Authenitication
{
    public class Role : IdentityRole<long>
    {
        public Role() : base() { }
        public Role(string roleName) : base(roleName) { }

        public virtual ICollection<UserRole> UserRoles { get; set; }
    }
}
