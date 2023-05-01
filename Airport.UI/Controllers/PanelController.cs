using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Airport.UI.Controllers
{
    public class PanelController : PanelAuthController
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Dashbord()
        {
            return View();
        }

        [HttpGet("panel/reservation-step-three")]
        public IActionResult ReservationStepThree()
        {
            return View();
        }
    }
}
