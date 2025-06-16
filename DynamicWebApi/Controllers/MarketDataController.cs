using DynamicWebApi.Models;
using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;
using System.Xml;

namespace DynamicWebApi.Controllers
{
    public class MarketDataController : Controller
    {
        private async Task<List<MarketDataModel>> GetMarketDataAsync()
        {
            var tcmbData = await GetTcmbMarketDataAsync();
            var cryptoData = await GetCryptoDataAsync();
            var metalData = await GetPreciousMetalDataFromBISTAsync();

            var combined = new List<MarketDataModel>();
            combined.AddRange(tcmbData);
            combined.AddRange(cryptoData);
            combined.AddRange(metalData);

            return combined;
        }

        public async Task<IActionResult> Index()
        {
            var allData = await GetMarketDataAsync();

            var viewModel = new MarketDataViewModel
            {
                Currencies = allData.Where(x => x.GroupName == "Döviz").ToList(),
                PreciousMetals = allData.Where(x => x.GroupName == "Değerli Madenler").ToList(),
                CryptoCurrencies = allData.Where(x => x.GroupName == "Kripto Paralar").ToList()
            };


            return View(viewModel);
        }

        private decimal ParseTcmbDecimal(string? input)
        {
            if (string.IsNullOrWhiteSpace(input)) return 0;
            input = input.Replace(",", ".");
            return decimal.TryParse(input, System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out var result) ? result : 0;
        }

        private async Task<List<MarketDataModel>> GetTcmbMarketDataAsync()
        {
            var list = new List<MarketDataModel>();
            var url = "https://www.tcmb.gov.tr/kurlar/today.xml";

            using (var client = new HttpClient())
            {
                var response = await client.GetStringAsync(url);
                var xmlDoc = XDocument.Parse(response);
                var dateAttr = xmlDoc.Root?.Attribute("Date")?.Value;

                foreach (var currency in xmlDoc.Descendants("Currency"))
                {
                    var code = currency.Attribute("CurrencyCode")?.Value;
                    if (string.IsNullOrWhiteSpace(code)) continue;

                    list.Add(new MarketDataModel
                    {
                        CurrencyCode = code,
                        CurrencyName = currency.Element("Isim")?.Value,
                        ForexBuying = ParseTcmbDecimal(currency.Element("ForexBuying")?.Value),
                        ForexSelling = ParseTcmbDecimal(currency.Element("ForexSelling")?.Value),
                        Unit = currency.Element("Unit")?.Value ?? "1",
                        Date = dateAttr,
                        SourceType = "TCMB",
                        GroupName = code is "XAU" or "XAG" ? "Değerli Madenler" : "Döviz"
                    });
                }

                // TRY sabit
                list.Add(new MarketDataModel
                {
                    CurrencyCode = "TRY",
                    CurrencyName = "Türk Lirası",
                    ForexBuying = 1,
                    ForexSelling = 1,
                    Unit = "1",
                    Date = dateAttr,
                    SourceType = "Sabit",
                    GroupName = "Döviz"
                });
            }

            return list;
        }

        private async Task<List<MarketDataModel>> GetCryptoDataAsync()
        {
            var list = new List<MarketDataModel>();
            var url = "https://api.coingecko.com/api/v3/simple/price?ids=bitcoin,ethereum,tether,ripple,cardano,solana,polkadot,dogecoin,chainlink,binancecoin&vs_currencies=try,usd";


            using (var client = new HttpClient())
            {
                var response = await client.GetStringAsync(url);
                var json = JObject.Parse(response);

                foreach (var coin in json)
                {
                    var id = coin.Key; // Örn: bitcoin
                    var values = coin.Value;

                    var code = id.ToUpper(); // BTC, ETH, vs.
                    var name = char.ToUpper(id[0]) + id.Substring(1); // Bitcoin, Ethereum, vs.

                    list.Add(new MarketDataModel
                    {
                        CurrencyCode = code,
                        CurrencyName = name,
                        ForexBuying = values["try"]?.Value<decimal>() ?? 0,
                        ForexSelling = values["try"]?.Value<decimal>() ?? 0,
                        Unit = "1",
                        Date = DateTime.Now.ToString("yyyy-MM-dd"),
                        SourceType = "CoinGecko",
                        GroupName = "Kripto Paralar"
                    });
                }
            }
            return list;
        }


        private decimal ParseBistDecimal(string? input)
        {
            if (string.IsNullOrWhiteSpace(input)) return 0;
            input = input.Replace(".", "").Replace(",", ".");
            return decimal.TryParse(input, System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out var result) ? result : 0;
        }

        private async Task<List<MarketDataModel>> GetPreciousMetalDataFromBISTAsync()
        {
            var list = new List<MarketDataModel>();
            var url = "https://www.borsaistanbul.com/datfile/kmtprfrnsxml";

            using (var handler = new HttpClientHandler { AllowAutoRedirect = true })
            using (var client = new HttpClient(handler))
            {
                // Bot engeline karşı User-Agent ve Accept ekleniyor
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64)");
                client.DefaultRequestHeaders.Add("Accept", "application/xml");

                string response;

                try
                {
                    response = await client.GetStringAsync(url);
                }
                catch (HttpRequestException ex)
                {
                    // Geriye boş liste döndür ve logla
                    Console.WriteLine("HTTP isteği başarısız: " + ex.Message);
                    return list;
                }

                XDocument xmlDoc;

                try
                {
                    xmlDoc = XDocument.Parse(response);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("XML parse hatası: " + ex.Message);
                    return list;
                }

                var date = xmlDoc.Descendants("gun").FirstOrDefault()?.Value;

                var metalMap = new Dictionary<string, string>
                {
                    { "altindeger", "Altın" },
                    { "gumusdeger", "Gümüş" },
                    { "platindeger", "Platin" },
                    { "paladyumdeger", "Paladyum" }
                };

                foreach (var kv in metalMap)
                {
                    var element = xmlDoc.Descendants(kv.Key).FirstOrDefault();
                    if (element == null) continue;

                    var price = ParseBistDecimal(element.Value);

                    list.Add(new MarketDataModel
                    {
                        CurrencyCode = kv.Key.ToUpper(),
                        CurrencyName = kv.Value,
                        ForexBuying = price,
                        ForexSelling = price,
                        Unit = "1", // TL/kg
                        Date = date,
                        SourceType = "BIST",
                        GroupName = "Değerli Madenler"
                    });
                }

                list.Add(new MarketDataModel
                {
                    CurrencyCode = "TRY",
                    CurrencyName = "Türk Lirası",
                    ForexBuying = 1,
                    ForexSelling = 1,
                    Unit = "1",
                    Date = date,
                    SourceType = "BIST",
                    GroupName = "Değerli Madenler"
                });
            }
            Console.WriteLine(list);
            return list;
        }


    }
}
