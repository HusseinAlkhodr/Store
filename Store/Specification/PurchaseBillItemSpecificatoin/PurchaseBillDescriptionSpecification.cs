using Store.Models.Invoice;

namespace Store.Specification.PurchaseBillItemSpecificatoin
{
    public class PurchaseBillDescriptionSpecification : BaseSpecification<PurchaseBillItem>
    {
        public PurchaseBillDescriptionSpecification(string? Description)
        {
            if (Description != null)
            {
                SetCriteria(a => a.Item.Description.Contains(Description));
            }
        }
    }
}
