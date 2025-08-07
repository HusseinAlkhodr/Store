using Store.Models.Invoice;

namespace Store.Specification.SaleReportSpecification
{
    public class SaleReportWeekSpecification : BaseSpecification<SaleBillItem>
    {
        public SaleReportWeekSpecification(DateTime? date)
        {
            var inputDate = date.Value.Date;
            var dayOfWeek = (int)inputDate.DayOfWeek;
            var daysToSubtract = (dayOfWeek + 1) % 7;
            var weekStart = inputDate.AddDays(-daysToSubtract);
            var weekEnd = weekStart.AddDays(6);
            SetCriteria(x => x.CreatedAt.Date >= weekStart && x.CreatedAt.Date <= weekEnd);
        }
    }
}
