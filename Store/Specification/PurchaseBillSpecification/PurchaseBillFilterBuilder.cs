using Store.Models.Invoice;

namespace Store.Specification.PurchaseBillSpecification
{
    public class PurchaseBillFilterBuilder : GenericFiltersBuilder<PurchaseBill, PurchaseBillFilterDTO>
    {
        public override void InitItemSpecifications()
        {
            AddSpecification(
                nameof(PurchaseBillFilterDTO.SourceName),
                filter => new PurchaseBillSourceSpecification(filter.SourceName)
                );
            AddSpecification(
                nameof(PurchaseBillFilterDTO.StratDate),
                filter => new PurchaseBillDateSpecification(filter.StratDate, filter.EndDate)
                );
        }
    }
}
