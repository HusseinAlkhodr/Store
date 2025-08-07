namespace Store.Models.Invoice
{
    public class SaleBill : BaseModel, ISoftDelete
    {
        public string? CustomerName { get; set; }
        public long Total { get; set; }
        public ICollection<SaleBillItem>? Items { get; set; }
        public int IsArchived { get; set; }
        public DateTime? ArchiveDate { get; set; }
        public long? ArchivedById { get; set; }
        public long ExchangeRate { get; set; }
    }
}
