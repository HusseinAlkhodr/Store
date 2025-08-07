using Store.Models.Invoice;

namespace Store.Specification.PurchaseBillItemSpecificatoin
{
    public class PurchaseBillItemFilterBuilder : GenericFiltersBuilder<PurchaseBillItem, PurchaseBillItemFilterDTO>
    {
        public override void InitItemSpecifications()
        {
            AddSpecification(
               nameof(PurchaseBillItemFilterDTO.Code),
               filter => new PurchaseBillCodeSpecification(filter.Code)
               );
            AddSpecification(
                nameof(PurchaseBillItemFilterDTO.Description),
                filter => new PurchaseBillDescriptionSpecification(filter.Description)
                );
            AddSpecification(
                nameof(PurchaseBillItemFilterDTO.Division),
                filter => new PurchaseBillDivisionSpecification(filter.Division)
                );
            AddSpecification(
                nameof(PurchaseBillItemFilterDTO.Vendor),
                filter => new PurchaseBillVendorSpecification(filter.Vendor)
                );
            AddSpecification(
                nameof(PurchaseBillItemFilterDTO.StartDate),
                filter => new PurchaseBillDateSpecification(filter.StartDate, filter.EndDate)
                );
        }
    }
}
