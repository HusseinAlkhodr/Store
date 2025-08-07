using Store.Models.Invoice;

namespace Store.Specification.PurchaseBillSpecification
{
    public class PurchaseBillDateSpecification : BaseSpecification<PurchaseBill>
    {
        public PurchaseBillDateSpecification(DateTime? StartDate, DateTime? EndDate)
        {
            if (StartDate != null)
            {
                SetCriteria(p => p.CreatedAt.Day >= StartDate.Value.Day && (EndDate == null || p.CreatedAt.Day <= EndDate.Value.Day));
            }
        }
    }
}
