using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace DynamicWebApi.Controllers
{
    public class WeatherController : Controller
    {
        private readonly HttpClient _client;
        private readonly string apiKey = "280723ba63a0507e6670e4df7486a76e";

        public WeatherController(IHttpClientFactory httpClientFactory)
        {
            _client = httpClientFactory.CreateClient();
        }

        [HttpGet]
        public async Task<IActionResult> GetWeather()
        {
            var url = $"https://api.openweathermap.org/data/2.5/weather?q=Istanbul&appid={apiKey}&units=metric&lang=tr";
            var response = await _client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                return Json(new { error = "API'dan veri alınamadı." });
            }

            var content = await response.Content.ReadAsStringAsync();
            return Content(content, "application/json");
        }


    }
}
