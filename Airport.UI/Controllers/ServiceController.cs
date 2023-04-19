using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Airport.UI.Controllers
{
    [Authorize]
    public class ServiceController : Controller
    {
        [Route("panel/service")]
        public IActionResult Index()
        {
            return View();
        }

        [Route("panel/addservice")]
        public IActionResult AddService()
        {
            return View();
        }
    }
}
