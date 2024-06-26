using Airport.DBEntitiesDAL.Interfaces;
using Airport.UI.Models.VM;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Claims;

namespace Airport.UI.ViewComponents;

public class Menu(IUserDatasDAL userDal, ILoginAuthDAL loginAuthDal, IDriversDAL driversDal) : ViewComponent
{

    public IViewComponentResult Invoke()
    {
        var userId = Convert.ToInt32(Request.HttpContext.User.Claims.Where(a => a.Type == ClaimTypes.Sid).Select(a => a.Value).SingleOrDefault());
        var auth = loginAuthDal.SelectByID(userId);
        if (auth != null)
        {
            var Recognizer = new RecognizerUserVM()
            {
                Driver = driversDal.SelectByID(auth.DriverId),
                User = userDal.SelectByID(auth.UserId),
                Type = auth.Type
            };

            if(Recognizer.Driver != null)
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
