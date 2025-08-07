using Store.Models.Invoice;
using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;

namespace Store.Models
{
    public class Item : BaseModel
    {
        public required string Code { get; set; }
        public required string Description { get; set; }
        public long QTY { get; set; }
        public long DivisionId { get; set; }
        [ForeignKey(nameof(DivisionId))]
        public Division? Division { get; set; }
        public long VendorId { get; set; }
        [ForeignKey(nameof(VendorId))]
        public Vendor? Vendor { get; set; }
        public long Cost { get; set; }
        public virtual ICollection<PriceList>? Prices { get; set; }
        public virtual ICollection<SaleBillItem>? SaleBillItems { get; set; }
        public virtual ICollection<PurchaseBillItem>? PurchaseBillItems { get; set; }
    }
}
