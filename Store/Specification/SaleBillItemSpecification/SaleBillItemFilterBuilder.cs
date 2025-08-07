using Store.Models.Invoice;

namespace Store.Specification.SaleBillItemSpecification
{
    public class SaleBillItemFilterBuilder : GenericFiltersBuilder<SaleBillItem, SaleBillItemFilterDTO>
    {
        public override void InitItemSpecifications()
        {
            AddSpecification(
                nameof(SaleBillItemFilterDTO.Code),
                filter => new SaleBillItemCodeSpecification(filter.Code)
                );
            AddSpecification(
                nameof(SaleBillItemFilterDTO.Description),
                filter => new SaleBillItemDescriptionSpecification(filter.Description)
                );
            AddSpecification(
                nameof(SaleBillItemFilterDTO.Division),
                filter => new SaleBillItemDivisionSpecification(filter.Division)
                );
            AddSpecification(
                nameof(SaleBillItemFilterDTO.Vendor),
                filter => new SaleBillItemVendorSpecification(filter.Vendor)
                );
            AddSpecification(
                nameof(SaleBillItemFilterDTO.StartDate),
                filter => new SaleBillItemDateSpecification(filter.StartDate, filter.EndDate)
                );
        }
    }
}
