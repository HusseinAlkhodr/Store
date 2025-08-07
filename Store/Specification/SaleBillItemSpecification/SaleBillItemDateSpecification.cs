using Store.Models.Invoice;

namespace Store.Specification.SaleBillItemSpecification
{
    public class SaleBillItemDateSpecification : BaseSpecification<SaleBillItem>
    {
        public SaleBillItemDateSpecification(DateTime? StartTime, DateTime? EndDate)
        {
            if (StartTime != null)
            {
                SetCriteria(a => a.CreatedAt >= StartTime && (EndDate == null || a.CreatedAt <= EndDate.Value.AddDays(1)));
            }
        }
    }
}
