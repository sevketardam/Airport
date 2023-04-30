using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Airport.UI.Controllers
{
    public class PanelController : BaseController
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Dashbord()
        {
            return View();
        }
    }
}
