using Microsoft.AspNetCore.Identity;
using Store.Core.Unit;
using Store.Middlewares;
using System.ComponentModel.DataAnnotations.Schema;

namespace Store.Models.Authenitication
{
    public enum AccountType
    {
        Admin = 1,
        User = 2
    }
    public enum AccountStatus
    {
        Active = 1,
        Suspend = 2
    }
    public class Account : IdentityUser<long>
    {
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; } = null;
        public long? CreatedById { get; set; }
        [ForeignKey(nameof(CreatedById))]
        public Account CreatedBy { get; set; }
        public long? UpdatedById { get; set; }
        [ForeignKey(nameof(UpdatedById))]
        public Account UpdatedBy { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public string FullName { get => FirstName + " " + LastName; }
        public bool IsApproved { get; set; }
        public AccountStatus Status { get; set; }
        public AccountType AccountType { get; set; }
        public virtual ICollection<UserClaim> UserClaims { get; set; }
        public virtual ICollection<UserRole> UserRoles { get; set; }
        public virtual ICollection<ItemType> ItemTypes { get; set; }
        public async Task Validiate(IUnitOfWork unitOfWork)
        {
            var EmailExists = await unitOfWork.AccountRepository
                .GetCount(x => x.Email.Equals(Email) && x.Id != Id) > 0;
            if (EmailExists)
                throw new HusseinErrorResponseException("Email Not Available");
        }
    }
}
