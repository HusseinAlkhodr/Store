using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Store.Models;
using Store.Models.Authenitication;
using Store.Models.Currency;
using Store.Models.Invoice;

namespace Store.DataBaseContext
{
    public class StoreDbContext : IdentityDbContext<Account, Role, long, IdentityUserClaim<long>, UserRole, IdentityUserLogin<long>, IdentityRoleClaim<long>, IdentityUserToken<long>>
    {
        public StoreDbContext(DbContextOptions<StoreDbContext> option) : base(option) { }

        public virtual DbSet<Account> Users { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<Division> Divisions { get; set; }
        public virtual DbSet<Vendor> Vendors { get; set; }
        public virtual DbSet<Item> Items { get; set; }
        public virtual DbSet<ItemType> ItemsType { get; set; }
        public virtual DbSet<PriceList> Prices { get; set; }
        public virtual DbSet<SaleBill> SaleBills { get; set; }
        public virtual DbSet<SaleBillItem> SaleBillItems { get; set; }
        public virtual DbSet<PurchaseBill> PurchaseBills { get; set; }
        public virtual DbSet<PurchaseBillItem> PurchaseBillItems { get; set; }
        public virtual DbSet<CurrencyExchangeRate> CurrenciesExchangeRates { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Account>().HasOne(e => e.CreatedBy).WithMany().HasForeignKey(a => a.CreatedById).OnDelete(DeleteBehavior.NoAction);
            builder.Entity<UserRole>(entity =>
            {
                entity.HasKey(ur => new { ur.UserId, ur.RoleId });
                entity.HasOne(ur => ur.User).WithMany(u => u.UserRoles).HasForeignKey(ur => ur.UserId).IsRequired();
                entity.HasOne(ur => ur.Role).WithMany(r => r.UserRoles).HasForeignKey(ur => ur.RoleId).IsRequired();

            });
            builder.Entity<UserClaim>(entity =>
            {
                entity.HasOne(e => e.User).WithMany(u => u.UserClaims).HasForeignKey(e => e.UserId).IsRequired();
            });


            builder.Entity<Division>().HasMany(d => d.Items).WithOne(i => i.Division).HasForeignKey(i => i.DivisionId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Vendor>().HasMany(v => v.Items).WithOne(i => i.Vendor).HasForeignKey(i => i.VendorId)
                .OnDelete(DeleteBehavior.NoAction);


            //--------------Item--------------//
            builder.Entity<Item>().HasMany(i => i.Prices).WithOne(p => p.Item).HasForeignKey(p => p.ItemId)
                .OnDelete(DeleteBehavior.NoAction);
            builder.Entity<Item>().HasMany(i => i.SaleBillItems).WithOne(s => s.Item).HasForeignKey(s => s.ItemId)
                .OnDelete(DeleteBehavior.NoAction);
            builder.Entity<Item>().HasMany(i => i.PurchaseBillItems).WithOne(p => p.Item).HasForeignKey(p => p.ItemId)
                .OnDelete(DeleteBehavior.NoAction);

            //--------------Bill-------------//
            builder.Entity<SaleBill>().HasMany(s => s.Items).WithOne(si => si.SaleBill).HasForeignKey(si => si.SaleBillId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<PurchaseBill>().HasMany(s => s.Items).WithOne(si => si.PurchaseBill).HasForeignKey(si => si.PurchaseBillId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<SaleBillItem>().HasOne(p => p.ItemType).WithMany().HasForeignKey(p => p.TypeId)
                .OnDelete(DeleteBehavior.NoAction);
            builder.Entity<PurchaseBillItem>().HasOne(p => p.ItemType).WithMany().HasForeignKey(p => p.TypeId)
                .OnDelete(DeleteBehavior.NoAction);
            //------------------------------//
            builder.Entity<ItemType>().HasOne(a => a.CreatedBy).WithMany(a => a.ItemTypes).HasForeignKey(t => t.CreatedById)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
