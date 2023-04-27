using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System;
using System.Security.Claims;
using Airport.DBEntitiesDAL.Interfaces;

namespace Airport.UI.ViewComponents
{
    public class ProfileSidebar : ViewComponent
    {
        IUserDatasDAL _user;
        public ProfileSidebar(IUserDatasDAL user)
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
