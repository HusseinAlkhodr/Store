using Store.Models.Invoice;

namespace Store.Specification.PurchaseBillItemSpecificatoin
{
    public class PurchaseBillDivisionSpecification : BaseSpecification<PurchaseBillItem>
    {
        public PurchaseBillDivisionSpecification(string? Division)
        {
            if (Division != null)
            {
                SetCriteria(a => a.Item.Division.Name.Contains(Division));
            }
        }
    }
}
