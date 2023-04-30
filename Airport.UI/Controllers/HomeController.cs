using Airport.DBEntities.Entities;
using Airport.DBEntitiesDAL.Interfaces;
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
        public HomeController(IUserDatasDAL userdata)
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


        //[HttpPost("getLocation")]
        //public IActionResult GetReservasionValue(GetResevationIM reservation,string reservationDatas)
        //{
        //    try
        //    {

        //        return RedirectToAction("ReservationStepTwo", "Reservation");
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}


        [HttpPost("register")]
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

        [HttpGet("manage-reservation")]
        public IActionResult ManageReseration()
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
