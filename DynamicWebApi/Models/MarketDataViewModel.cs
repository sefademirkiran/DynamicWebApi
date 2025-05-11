namespace DynamicWebApi.Models
{
    public class MarketDataViewModel
    {
        public List<MarketDataModel> Currencies { get; set; } = new();
        public List<MarketDataModel> PreciousMetals { get; set; } = new();
        public List<MarketDataModel> CryptoCurrencies { get; set; } = new();
    }

}
