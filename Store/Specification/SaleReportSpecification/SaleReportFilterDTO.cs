namespace Store.Specification.SaleReportSpecification
{
    public class SaleReportFilterDTO
    {
        public string? Name { get; set; }
        public DateTime? StartDate { get; set; } = DateTime.UtcNow;
    }
}
