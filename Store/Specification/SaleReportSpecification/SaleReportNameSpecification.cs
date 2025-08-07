using Store.Models.Invoice;

namespace Store.Specification.SaleReportSpecification
{
    public class SaleReportNameSpecification : BaseSpecification<SaleBillItem>
    {
        public SaleReportNameSpecification(string? Name)
        {
            if (Name != null)
            {
                SetCriteria(x => x.Item.Description.Contains(Name));
            }
        }
    }
}
