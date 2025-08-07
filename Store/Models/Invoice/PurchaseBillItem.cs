using System.ComponentModel.DataAnnotations.Schema;

namespace Store.Models.Invoice
{
    public class PurchaseBillItem : BaseModel
    {
        public long ItemId { get; set; }
        [ForeignKey(nameof(ItemId))]
        public Item? Item { get; set; }
        public long PurchaseBillId { get; set; }
        [ForeignKey(nameof(PurchaseBillId))]
        public PurchaseBill? PurchaseBill { get; set; }
        public long TypeId { get; set; }
        [ForeignKey(nameof(TypeId))]
        public ItemType? ItemType { get; set; }
        public int QTY { get; set; }
        public long ItemCost { get; set; }
        public long TotalCost { get; set; }
    }
}
