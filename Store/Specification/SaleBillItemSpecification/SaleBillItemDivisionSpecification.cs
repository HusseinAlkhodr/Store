using Store.Models.Invoice;

namespace Store.Specification.SaleBillItemSpecification
{
    public class SaleBillItemDivisionSpecification : BaseSpecification<SaleBillItem>
    {
        public SaleBillItemDivisionSpecification(string? division)
        {
            if (division != null)
            {
                SetCriteria(i => i.Item.Division.Name.Contains(division));
            }
        }
    }
}
