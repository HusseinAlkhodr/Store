using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Store.Models.Authenitication
{
    public class UserClaim : IdentityUserClaim<long>
    {
        [ForeignKey(nameof(Id))]
        public Account User { get; set; }
    }
}
