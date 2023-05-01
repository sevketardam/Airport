using Airport.DBEntitiesDAL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Claims;

namespace Airport.UI.ViewComponents
{
    public class Menu : ViewComponent
    {
        IUserDatasDAL _user;
        public Menu(IUserDatasDAL user)
        {
            _user = user;
        }

        public IViewComponentResult Invoke()
        {
            var userId = Convert.ToInt32(Request.HttpContext.User.Claims.Where(a => a.Type == ClaimTypes.Sid).Select(a => a.Value).SingleOrDefault());
            var user = _user.SelectByID(userId);
            if (user != null)
            {
                return View(user);
            }
            return null;
        }
    }
}
