using Store.Models.Invoice;

namespace Store.Specification.SaleBillSpecification
{
    public class SaleBillDateSpecification : BaseSpecification<SaleBill>
    {
        public SaleBillDateSpecification(DateTime? StartDate, DateTime? EndDate)
        {
            if (StartDate != null)
            {
                SetCriteria(s => s.CreatedAt.Day >= StartDate.Value.Day && (EndDate == null || s.CreatedAt.Day <= EndDate.Value.Day));
            }
        }
    }
}
