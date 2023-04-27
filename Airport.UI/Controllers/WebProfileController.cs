using Airport.DBEntitiesDAL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Airport.UI.Controllers
{
    public class WebProfileController : BaseController
    {
        IUserDatasDAL _user;
        public WebProfileController(IUserDatasDAL user)
        {
            _user = user;
        }

        [HttpGet("profile")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("profile/settings")]
        public IActionResult Settings()
        {
            return View();
        }

        [HttpGet("profile/change-password")]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpGet("profile/bank-account")]
        public IActionResult BankAccount()
        {
            return View();
        }

        [HttpGet("profile/agreements")]
        public IActionResult Agreement()
        {
            return View();
        }

        [HttpGet("profile/my-company")]
        public IActionResult Company()
        {
            return View();
        }
    }
}
