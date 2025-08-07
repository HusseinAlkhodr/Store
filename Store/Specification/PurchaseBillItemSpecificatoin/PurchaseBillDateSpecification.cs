using Store.Models.Invoice;

namespace Store.Specification.PurchaseBillItemSpecificatoin
{
    public class PurchaseBillDateSpecification : BaseSpecification<PurchaseBillItem>
    {
        public PurchaseBillDateSpecification(DateTime? StartDate, DateTime? EndDate)
        {
            if (StartDate != null)
            {
                SetCriteria(a => a.CreatedAt >= StartDate && (EndDate == null || a.CreatedAt <= EndDate.Value.AddDays(1)));
            }
        }
    }
}
