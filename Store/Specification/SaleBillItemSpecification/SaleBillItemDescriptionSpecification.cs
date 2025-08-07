using Store.Models.Invoice;

namespace Store.Specification.SaleBillItemSpecification
{
    public class SaleBillItemDescriptionSpecification : BaseSpecification<SaleBillItem>
    {
        public SaleBillItemDescriptionSpecification(string? Description)
        {
            if (Description != null)
            {
                SetCriteria(i => i.Item.Description.Contains(Description));
            }
        }
    }
}
