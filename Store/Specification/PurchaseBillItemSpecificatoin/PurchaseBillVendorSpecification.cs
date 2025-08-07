using Store.Models.Invoice;

namespace Store.Specification.PurchaseBillItemSpecificatoin
{
    public class PurchaseBillVendorSpecification : BaseSpecification<PurchaseBillItem>
    {
        public PurchaseBillVendorSpecification(string? Vendor)
        {
            if (Vendor != null)
            {
                SetCriteria(a => a.Item.Vendor.Name.Contains(Vendor));
            }
        }
    }
}
