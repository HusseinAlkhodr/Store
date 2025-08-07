using Store.Models.Invoice;

namespace Store.Specification.SaleBillSpecification

{
    public class SaleBillFilterBuilder : GenericFiltersBuilder<SaleBill, SaleBillFilterDTO>
    {
        public override void InitItemSpecifications()
        {
            AddSpecification(
                nameof(SaleBillFilterDTO.CustomerName),
                filter => new SaleBillCustomerSpecification(filter.CustomerName)
            );
            AddSpecification(
                nameof(SaleBillFilterDTO.StartDate),
                filter => new SaleBillDateSpecification(filter.StartDate, filter.EndDate));
        }
    }
}
