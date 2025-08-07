using Store.Models.Invoice;

namespace Store.Specification.SaleBillItemSpecification
{
    public class SaleBillItemCodeSpecification : BaseSpecification<SaleBillItem>
    {
        public SaleBillItemCodeSpecification(string? Code)
        {
            if (Code != null)
            {
                SetCriteria(a => a.Item.Code == Code);
            }
        }
    }
}
