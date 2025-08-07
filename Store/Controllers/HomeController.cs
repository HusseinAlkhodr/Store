using Microsoft.AspNetCore.Mvc;
using Store.Core.Unit;
using Store.Models;
using Store.Models.Currency;
using System.Diagnostics;

namespace Store.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            this.unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            var rate = (await unitOfWork.CurrencyExchangeRateRepository.GetAll(orderBy: o => o.OrderByDescending(a => a.CreatedAt))).Select(a => a.ExchangeRate).FirstOrDefault();
            return View(rate);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeRate([FromForm] decimal rate, [FromForm] string returnUrl)
        {
            var oldRate = (await unitOfWork.CurrencyExchangeRateRepository
                .GetAll(orderBy: o => o.OrderByDescending(a => a.CreatedAt)))
                .Select(a => a.ExchangeRate).FirstOrDefault();
            await unitOfWork.CurrencyExchangeRateRepository.Insert(new CurrencyExchangeRate
            {
                ExchangeRate = (long)rate,
                CreatedAt = DateTime.UtcNow.ToLocalTime(),
                CreatedById = (long)CurrentUserId
            });
            var items = await unitOfWork.ItemRepository.GetAll();
            foreach (var item in items)
            {
                var oldPrice = (await unitOfWork.PriceListRepository.GetAll(
                    a => a.ItemId == item.Id,
                    orderBy: o => o.OrderByDescending(i => i.CreatedAt)))
                    .Select(p => p.Price).FirstOrDefault();
                var NewPrice = ((decimal)oldPrice / (decimal)oldRate) * (decimal)rate;
                await unitOfWork.PriceListRepository.Insert(new PriceList
                {
                    CreatedAt = DateTime.UtcNow.ToLocalTime(),
                    CreatedById = (long)CurrentUserId,
                    ItemId = item.Id,
                    ExchangeRate = (long)rate,
                    Price = (long)NewPrice
                });
            }
            await unitOfWork.Save();
            return Redirect(returnUrl ?? "/");
        }
        [HttpGet]
        public async Task<IActionResult> GetRate()
        {
            var rate = (await unitOfWork.CurrencyExchangeRateRepository
                .GetAll(orderBy: o => o.OrderByDescending(i => i.CreatedAt)))
                .Select(i => i.ExchangeRate).FirstOrDefault();
            return Ok(rate);
        }
    }
}
