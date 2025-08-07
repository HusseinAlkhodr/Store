namespace Store.Models.Invoice
{
    public class PurchaseBill : BaseModel, ISoftDelete
    {
        public string? CustomerName { get; set; }
        public long Total { get; set; }
        public ICollection<PurchaseBillItem>? Items { get; set; }
        public int IsArchived { get; set; }
        public DateTime? ArchiveDate { get; set; }
        public long? ArchivedById { get; set; }
        public long ExchangeRate { get; set; }
    }
}
