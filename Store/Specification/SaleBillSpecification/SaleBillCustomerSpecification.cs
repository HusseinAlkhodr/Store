using Store.Models.Invoice;

namespace Store.Specification.SaleBillSpecification
{
    public class SaleBillCustomerSpecification : BaseSpecification<SaleBill>
    {
        public SaleBillCustomerSpecification(string? name)
        {
            if (name != null)
            {
                SetCriteria(a => a.CustomerName.Contains(name));
            }
        }
    }
}
