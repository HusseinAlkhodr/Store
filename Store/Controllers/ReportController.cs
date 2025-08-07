using Microsoft.AspNetCore.Mvc;
using Store.Core.Unit;
using Store.DTO;
using Store.Models.Invoice;
using Store.Specification.SaleReportSpecification;

namespace Store.Controllers
{
    public class ReportController : BaseController
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly SaleReportFilterBuilder dailyFilterBuilder;
        private readonly SaleReportMonthlyFilterBuilder MonthlyfilterBuilder;
        private readonly SaleReportWeeklyFilterBuilder weeklyFilterBuilder;

        public ReportController(
            IUnitOfWork unitOfWork,
            SaleReportFilterBuilder filterBuilder,
            SaleReportMonthlyFilterBuilder filterBuilder1,
            SaleReportWeeklyFilterBuilder filterBuilder2)
        {
            this.unitOfWork = unitOfWork;
            this.dailyFilterBuilder = filterBuilder;
            this.MonthlyfilterBuilder = filterBuilder1;
            this.weeklyFilterBuilder = filterBuilder2;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> GetDailyReport([FromQuery] SaleReportFilterDTO filterDTO)
        {
            filterDTO.StartDate = filterDTO.StartDate == null ? DateTime.UtcNow.ToLocalTime().Date : filterDTO.StartDate;
            var specification = unitOfWork.SaleBillItemRepository.InjectSpecification(dailyFilterBuilder, filterDTO);
            var sales = await unitOfWork.SaleBillItemRepository.GetAll(
                includeProperties: nameof(SaleBillItem.Item) + "," + nameof(SaleBillItem.ItemType) + "," + nameof(SaleBillItem.SaleBill),
                extendQuery: specification,
                filter: a => a.SaleBill.IsArchived != 1
            );

            var groupedSales = sales
                .GroupBy(s => s.Item.Description)
                .Select(g =>
                {
                    return new ItemSale
                    {
                        Description = g.Key,
                        QTY = g.Sum(x => x.QTY * x.ItemType.QTY),
                        Total = g.Sum(x => x.TotalPrice),
                        Rest = g.First().Item.QTY  // استخدم البيانات الموجودة مسبقًا
                    };
                }).ToList();


            var report = new SalesReport
            {
                Date = (DateTime)filterDTO.StartDate,
                Sales = groupedSales.ToList(),
                Total = groupedSales.Sum(x => x.Total)
            };

            return View(report); // وتأكد من أن @model هو DailySaleReport
        }
        public async Task<IActionResult> GetMonthlyReport([FromQuery] SaleReportFilterDTO filterDTO)
        {
            filterDTO.StartDate = filterDTO.StartDate == null ? DateTime.UtcNow.Date : filterDTO.StartDate;
            var specification = unitOfWork.SaleBillItemRepository.InjectSpecification(MonthlyfilterBuilder, filterDTO);
            var sales = await unitOfWork.SaleBillItemRepository.GetAll(
                includeProperties: nameof(SaleBillItem.Item) + "," + nameof(SaleBillItem.ItemType) + "," + nameof(SaleBillItem.SaleBill),
                extendQuery: specification,
                filter: a => a.SaleBill.IsArchived != 1
            );

            var groupedSales = sales
                .GroupBy(s => s.Item.Description)
                .Select(g =>
                {
                    return new ItemSale
                    {
                        Description = g.Key,
                        QTY = g.Sum(x => x.QTY * x.ItemType.QTY),
                        Total = g.Sum(x => x.TotalPrice),
                        Rest = g.First().Item.QTY  // استخدم البيانات الموجودة مسبقًا
                    };
                }).ToList();


            var report = new SalesReport
            {
                Date = (DateTime)filterDTO.StartDate,
                Sales = groupedSales.ToList(),
                Total = groupedSales.Sum(x => x.Total)
            };

            return View(report); // وتأكد من أن @model هو DailySaleReport
        }
        public async Task<IActionResult> GetWeeklyReport([FromQuery] SaleReportFilterDTO filterDTO)
        {
            filterDTO.StartDate = filterDTO.StartDate == null ? DateTime.UtcNow.Date : filterDTO.StartDate;
            var specification = unitOfWork.SaleBillItemRepository.InjectSpecification(weeklyFilterBuilder, filterDTO);
            var sales = await unitOfWork.SaleBillItemRepository.GetAll(
                includeProperties: nameof(SaleBillItem.Item) + "," + nameof(SaleBillItem.ItemType) + "," + nameof(SaleBillItem.SaleBill),
                extendQuery: specification,
                filter: a => a.SaleBill.IsArchived != 1
            );

            var groupedSales = sales
                .GroupBy(s => s.Item.Description)
                .Select(g =>
                {
                    return new ItemSale
                    {
                        Description = g.Key,
                        QTY = g.Sum(x => x.QTY * x.ItemType.QTY),
                        Total = g.Sum(x => x.TotalPrice),
                        Rest = g.First().Item.QTY  // استخدم البيانات الموجودة مسبقًا
                    };
                }).ToList();


            var report = new SalesReport
            {
                Date = (DateTime)filterDTO.StartDate,
                Sales = groupedSales.ToList(),
                Total = groupedSales.Sum(x => x.Total)
            };

            return View(report); // وتأكد من أن @model هو DailySaleReport
        }
    }
}
