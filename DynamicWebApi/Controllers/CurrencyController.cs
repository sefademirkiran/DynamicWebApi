using Microsoft.AspNetCore.Mvc;

namespace DynamicWebApi.Controllers
{
    public class CurrencyController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
