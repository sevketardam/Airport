using Airport.DBEntities.Entities;
using Airport.DBEntitiesDAL.Interfaces;
using Airport.UI.Models;
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
        public HomeController(IUserDatasDAL userdata)
        {
            _userdata = userdata;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Route("about")]
        public IActionResult About()
        {
            return View();
        }

        [Route("contact")]
        public IActionResult Contact()
        {
            return View();
        }

        [Route("agencies")]
        public IActionResult Agencies()
        {
            return View();
        }

        [Route("privacy")]
        public IActionResult Privacy()
        {
            return View();
        }

        [Route("register")]
        public IActionResult Register()
        {
            return View();
        }

        [Route("terms")]
        public IActionResult Terms()
        {
            return View();
        }


        [Route("register")]
        [HttpPost]
        public IActionResult Register(UserDatas user)
        {
            var checkUser = _userdata.SelectByFunc(a=>a.Eposta == user.Eposta).FirstOrDefault();
            if (checkUser == null)
            {
                user.Type = 1;
                user.Password = GetMD5(user.Password);
                _userdata.Insert(user);

                return RedirectToAction("Index", "Home");
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
    }
}
