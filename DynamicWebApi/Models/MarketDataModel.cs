namespace DynamicWebApi.Models
{
    public class MarketDataModel
    {
        public string CurrencyCode { get; set; }
        public string CurrencyName { get; set; }
        public decimal ForexBuying { get; set; }
        public decimal? ForexSelling { get; set; }
        public decimal BanknoteBuying { get; set; }
        public decimal BanknoteSelling { get; set; }
        public string Unit { get; set; }
        public string Date { get; set; }

        public string? SourceType { get; set; }
        public string? GroupName { get; set; }

    }
}
