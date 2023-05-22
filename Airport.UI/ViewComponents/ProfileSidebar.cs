using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System;
using System.Security.Claims;
using Airport.DBEntitiesDAL.Interfaces;
using Airport.DBEntities.Entities;
using Airport.UI.Models.VM;

namespace Airport.UI.ViewComponents
{
    public class ProfileSidebar : ViewComponent
    {
        IUserDatasDAL _user;
        ILoginAuthDAL _loginAuth;
        IDriversDAL _drivers;
        public ProfileSidebar(IUserDatasDAL user, ILoginAuthDAL loginAuth, IDriversDAL drivers)
        {
            _user = user;
            _loginAuth = loginAuth;
            _drivers = drivers;
        }

        public IViewComponentResult Invoke()
        {
            var userId = Convert.ToInt32(Request.HttpContext.User.Claims.Where(a => a.Type == ClaimTypes.Sid).Select(a => a.Value).SingleOrDefault());
            var auth = _loginAuth.SelectByID(userId);
            if (auth != null)
            {
                var Recognizer = new RecognizerUserVM()
                {
                    Driver = _drivers.SelectByID(auth.DriverId),
                    User = _user.SelectByID(auth.UserId),
                    Type = auth.Type
                };

                if (Recognizer.Driver != null)
                {
                    Recognizer.Driver.LoginAuth = auth;
                    Recognizer.Name = Recognizer.Driver.Name;
                }
                else
                {
                    Recognizer.User.LoginAuth = auth;
                    Recognizer.Name = Recognizer.User.Name;
                }
                return View(Recognizer);
            }
            return null;
        }
    }
}
