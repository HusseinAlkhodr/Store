namespace Store.Specification.PurchaseBillItemSpecificatoin
{
    public class PurchaseBillItemFilterDTO
    {
        public string? Code { get; set; }
        public string? Description { get; set; }
        public string? Division { get; set; }
        public string? Vendor { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
