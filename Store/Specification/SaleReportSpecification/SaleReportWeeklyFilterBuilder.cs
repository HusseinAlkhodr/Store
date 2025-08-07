using Store.Models.Invoice;

namespace Store.Specification.SaleReportSpecification
{
    public class SaleReportWeeklyFilterBuilder : GenericFiltersBuilder<SaleBillItem, SaleReportFilterDTO>
    {
        public override void InitItemSpecifications()
        {
            AddSpecification(
                nameof(SaleReportFilterDTO.Name),
                filter => new SaleReportNameSpecification(filter.Name)
                );
            AddSpecification(
                nameof(SaleReportFilterDTO.StartDate),
                filter => new SaleReportWeekSpecification(filter.StartDate)
                );
        }
    }
}
