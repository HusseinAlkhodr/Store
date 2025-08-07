using Store.Models.Invoice;

namespace Store.Specification.SaleBillItemSpecification
{
    public class SaleBillItemVendorSpecification : BaseSpecification<SaleBillItem>
    {
        public SaleBillItemVendorSpecification(string? Vendor)
        {
            if (Vendor != null)
            {
                SetCriteria(i => i.Item.Vendor.Name.Contains(Vendor));
            }
        }
    }
}
