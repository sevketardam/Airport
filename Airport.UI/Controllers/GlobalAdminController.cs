using Microsoft.AspNetCore.Mvc;

namespace Airport.UI.Controllers
{
    public class GlobalAdminController : Controller
    {
        public IActionResult CreateCoupon()
        {
            return View();
        }
    }
}
