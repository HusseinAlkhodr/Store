using System.ComponentModel.DataAnnotations.Schema;

namespace Store.Models.Invoice
{
    public class SaleBillItem : BaseModel
    {
        public long ItemId { get; set; }
        [ForeignKey(nameof(ItemId))]
        public Item? Item { get; set; }
        public long SaleBillId { get; set; }
        [ForeignKey(nameof(SaleBillId))]
        public SaleBill? SaleBill { get; set; }
        public long TypeId { get; set; }
        [ForeignKey(nameof(TypeId))]
        public ItemType? ItemType { get; set; }
        public int QTY { get; set; }
        public long ItemPrice { get; set; }
        public long TotalPrice { get; set; }
    }
}
