using Airport.DBEntities.Entities;
using Airport.DBEntitiesDAL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Cryptography;

namespace Airport.UI.Controllers;

public class HomeController(IUserDatasDAL userdataDal, ILoginAuthDAL loginAuthDal) : Controller
{

    public IActionResult Index() => View();

    [HttpGet("about")]
    public IActionResult About() => View();

    [HttpGet("contact")]
    public IActionResult Contact() => View();

    [HttpGet("privacy")]
    public IActionResult Privacy() => View();

    [HttpGet("cookie-policy")]
    public IActionResult CookiePolicy() => View();

    [HttpGet("cancellation-policy")]
    public IActionResult CancellationPolicy() => View();

    [HttpGet("register")]
    public IActionResult Register()
    {
        if (!User.Identity.IsAuthenticated)
        {
            return View();
        }
        return RedirectToAction("Index", "Home");
    }

    [HttpGet("terms")]
    public IActionResult Terms() => View();

    [HttpPost("register")]
    public IActionResult Register(UserDatas user, string Eposta, string Password)
    {
        try
        {
            var checkUser = loginAuthDal.SelectByFunc(a => a.Email == Eposta).FirstOrDefault();
            if (checkUser == null)
            {
                var addedUser = userdataDal.Insert(new UserDatas
                {
                    Name = user.Name,
                    PhoneNumber = user.PhoneNumber,
                    RealPhone = user.RealPhone,
                    Img = "/img/airglobal-icon-black.png"
                });

                loginAuthDal.Insert(new LoginAuth
                {
                    Email = Eposta,
                    Password = GetMD5(Password),
                    Type = 1,
                    DriverId = 0,
                    UserId = addedUser.Id
                });

                return Json(new { result = 1 });
            }
            return Json(new { result = 2 });
        }
        catch (Exception)
        {
            return Json(new { });
        }
    }

    public static string GetMD5(string value)
    {
        MD5 md5 = MD5.Create();
        byte[] md5Bytes = System.Text.Encoding.Default.GetBytes(value);
        byte[] cryString = md5.ComputeHash(md5Bytes);
        string md5Str = string.Empty;
        for (int i = 0; i < cryString.Length; i++)
        {
            md5Str += cryString[i].ToString("X");
        }
        return md5Str;
    }

    [HttpGet("manage-reservation")]
    public IActionResult ManageReservation() => View();

    [HttpGet("reservation-information")]
    public IActionResult ReservationInformation() => View();

}
