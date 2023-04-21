using Microsoft.AspNetCore.Mvc;

namespace Airport.UI.Controllers
{
    public class CarsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
