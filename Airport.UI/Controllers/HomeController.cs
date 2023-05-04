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
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Airport.UI.Controllers
{
    public class HomeController : Controller
    {
        IUserDatasDAL _userdata;
        public HomeController(IUserDatasDAL userdata,IMail mail)
        {
            _userdata = userdata;
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

        [HttpPost("agencies",Name ="agenciesForm")]
        public IActionResult Agencies(UserDatas data)
        {
            var datas = _userdata.SelectByFunc(a => a.Eposta == data.Eposta).FirstOrDefault();
            if (datas == null)
            {
                var newUser = new UserDatas
                {
                    Linkedin = data.Linkedin,
                    Name = data.Name,
                    AboutUs = data.AboutUs,
                    Address = data.Address,
                    CompanyEmail = data.CompanyEmail,
                    CompanyName = data.CompanyName,
                    CompanyPhoneNumber = data.CompanyPhoneNumber,
                    CompanyWebsite = data.CompanyWebsite,
                    Eposta = data.Eposta,
                    Facebook = data.Facebook,
                    Password = GetMD5(data.Password),
                    Type = 2,
                    Profession = data.Profession,
                    TransferRequest = data.TransferRequest,
                    TransferRequestLocation = data.TransferRequestLocation,
                    PhoneNumber = data.PhoneNumber,
                };

                _userdata.Insert(newUser);

                ViewBag.Message = "success";
            }

            ViewBag.Message = "have";
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

        [HttpPost("register",Name = "personRegister")]
        public IActionResult Register(UserDatas user)
        {
            var checkUser = _userdata.SelectByFunc(a=>a.Eposta == user.Eposta).FirstOrDefault();
            if (checkUser == null)
            {
                user.Type = 1;
                user.Password = GetMD5(user.Password);
                _userdata.Insert(user);


                ViewBag.Message = "success";
                return View();
            }

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

    }
}
