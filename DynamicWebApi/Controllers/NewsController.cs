using DynamicWebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Xml.Linq;

namespace DynamicWebApi.Controllers
{
    public class NewsController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string apiKey = "50846e55d2955f14cf174449c2b7bbca";

        public NewsController(IHttpClientFactory factory)
        {
            _httpClient = factory.CreateClient();
        }

        public async Task<IActionResult> Index()
        {
            // GNews verisi
            string gnewsUrl = $"https://gnews.io/api/v4/top-headlines?lang=en&max=6&token={apiKey}";
            var gnewsResponse = await _httpClient.GetStringAsync(gnewsUrl);
            var gnewsJson = JObject.Parse(gnewsResponse);
            var gnewsArticles = gnewsJson["articles"];

            // RSS verisi
            string rssUrl = "https://www.hurriyet.com.tr/rss/anasayfa";
            var xml = await _httpClient.GetStringAsync(rssUrl); 
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
                .Take(21)
                .ToList();

            ViewBag.RssHaberler = rssItems;
            return View(gnewsArticles);
        }
    }
}



