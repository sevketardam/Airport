using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System;

namespace Airport.UI.ViewComponents
{
    public class ProfileSidebar : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
