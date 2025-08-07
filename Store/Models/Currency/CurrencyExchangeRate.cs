using System.ComponentModel.DataAnnotations.Schema;

namespace Store.Models.Currency
{
    public class CurrencyExchangeRate : BaseModel
    {
        public long ExchangeRate {  get; set; }
    }
}
