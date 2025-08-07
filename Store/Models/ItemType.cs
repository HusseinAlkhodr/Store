using Store.Models.Invoice;

namespace Store.Models
{
    public class ItemType : BaseModel
    {
        public required string Name { get; set; }
        public int QTY { get; set; }
        public ICollection<Item>? Items { get; set; }
        public ICollection<SaleBillItem>? SaleBillItems { get; set; }
        public ICollection<PurchaseBillItem>? PurchaseBillItems { get; set; }
    }
}
