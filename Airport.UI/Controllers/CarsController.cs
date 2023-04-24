using Microsoft.AspNetCore.Mvc;

namespace Airport.UI.Controllers
{
    public class CarsController : Controller
    {
        [Route("panel/mycars")]
        public IActionResult Index()
        {
            return View();
        }


        [Route("panel/mycars")]
        public IActionResult AddMyCar()
        {
            return View();
        }
    }
}
