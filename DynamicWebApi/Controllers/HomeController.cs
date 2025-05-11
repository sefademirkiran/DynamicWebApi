using DynamicWebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DynamicWebApi.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HttpClient _client;
        private readonly string apiKey = "50846e55d2955f14cf174449c2b7bbca";

        public HomeController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _client = httpClientFactory.CreateClient();
        }

        public async Task<IActionResult> Index()
        {
            string rssUrl = "https://www.hurriyet.com.tr/rss/anasayfa";
            var httpClient = new HttpClient();
            var xml = await httpClient.GetStringAsync(rssUrl);
            XDocument rssXml = XDocument.Parse(xml);

            XNamespace media = "http://search.yahoo.com/mrss/";

            var rssItems = rssXml.Descendants("item")
                .Select(item => new RssNewsModel
                {
                    Title = item.Element("title")?.Value,
                    Description = item.Element("description")?.Value,
                    Link = item.Element("link")?.Value,
                    PubDate = item.Element("pubDate")?.Value,
                    Image = item.Element(media + "thumbnail")?.Attribute("url")?.Value
                })
                .Take(3) // 🔥 sadece 3 haber
                .ToList();

            ViewBag.PreviewNews = rssItems;

            return View();
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
    }
}
