using Store.Models.Invoice;

namespace Store.Specification.PurchaseBillItemSpecificatoin
{
    public class PurchaseBillCodeSpecification : BaseSpecification<PurchaseBillItem>
    {
        public PurchaseBillCodeSpecification(string? Code)
        {
            if (Code != null)
            {
                SetCriteria(a => a.Item.Code == Code);
            }
        }
    }
}
