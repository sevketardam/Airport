using Microsoft.AspNetCore.Mvc;

namespace Airport.UI.Controllers
{
    public class LocationController : Controller
    {
        [HttpGet("panel/location")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("panel/add-location")]
        public IActionResult AddLocation()
        {
            return View();
        }

    }
}
