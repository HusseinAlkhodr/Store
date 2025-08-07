using Store.Models.Invoice;

namespace Store.Specification.SaleReportSpecification
{
    public class SaleReportDateSpecification : BaseSpecification<SaleBillItem>
    {
        public SaleReportDateSpecification(DateTime? Date)
        {
            if (Date != null)
            {
                SetCriteria(x => x.SaleBill.CreatedAt.Day == Date.Value.Day);
            }
        }
    }
}
