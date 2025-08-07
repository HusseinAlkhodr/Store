using Store.Models.Invoice;

namespace Store.Specification.PurchaseBillSpecification
{
    public class PurchaseBillSourceSpecification : BaseSpecification<PurchaseBill>
    {
        public PurchaseBillSourceSpecification(string? SourceName)
        {
            if (SourceName != null)
            {
                SetCriteria(p => p.CustomerName.Contains(SourceName));
            }
        }
    }
}
