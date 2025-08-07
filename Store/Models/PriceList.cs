using System.ComponentModel.DataAnnotations.Schema;

namespace Store.Models
{
    public class PriceList : BaseModel
    {
        public long Price { get; set; }
        public long ExchangeRate { get; set; }
        public long ItemId { get; set; }
        [ForeignKey(nameof(ItemId))]
        public Item Item { get; set; }
    }
}
