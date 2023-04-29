using Airport.UI.Models.IM;
using Microsoft.AspNetCore.Mvc;

namespace Airport.UI.Controllers
{
    public class ReservationController : Controller
    {
        public ReservationController()
        {
            
        }

        [HttpPost("reservation", Name = "getLocationValue")]
        public IActionResult ReservationStepTwo(GetResevationIM reservation)
        {
            return View(reservation);
        }
    }
}
