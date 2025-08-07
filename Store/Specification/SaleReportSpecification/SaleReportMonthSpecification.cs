using Store.Models.Invoice;

namespace Store.Specification.SaleReportSpecification
{
    public class SaleReportMonthSpecification : BaseSpecification<SaleBillItem>
    {
        public SaleReportMonthSpecification(DateTime? Date)
        {
            if (Date != null)
            {
                SetCriteria(x => x.CreatedAt.Date.Month == Date.Value.Month);
            }
        }
    }
}
