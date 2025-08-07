using Microsoft.AspNetCore.Mvc;
using Store.Core.Unit;

namespace Store.ViewComponents
{
    public class ExchangeRateViewComponent : ViewComponent
    {
        private readonly IUnitOfWork unitOfWork;

        public ExchangeRateViewComponent(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var rate = (await unitOfWork.CurrencyExchangeRateRepository
                .GetAll(orderBy: o => o.OrderByDescending(a => a.CreatedAt)))
                .Select(a => a.ExchangeRate).FirstOrDefault();

            return View("Default", Convert.ToDecimal(rate));
        }
    }
}
