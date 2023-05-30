using Airport.DBEntitiesDAL.Interfaces;
using Airport.UI.Models.VM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System;
using System.Security.Claims;
using Airport.UI.Models.Interface;
using Airport.DBEntities.Entities;
using System.Security.Cryptography;

namespace Airport.UI.Controllers
{
    public class PanelController : PanelAuthController
    {
        IMyCarsDAL _myCars;
        IUserDatasDAL _userDatas;
        ILocationsDAL _locations;
        IGetCarDetail _getCarDetail;
        IReservationsDAL _reservations;
        ILoginAuthDAL _loginAuth;
        IUserDatasDAL _user;
        public PanelController(IMyCarsDAL myCars, IUserDatasDAL userDatas, ILocationsDAL locations, IGetCarDetail getCarDetail,IReservationsDAL reservations, ILoginAuthDAL loginAuth,IUserDatasDAL user)
        {
            _myCars = myCars;
            _userDatas = userDatas;
            _locations = locations;
            _getCarDetail = getCarDetail;
            _reservations = reservations;   
            _loginAuth = loginAuth;
            _user = user;
        }

        public IActionResult Index()
        {
            try
            {
                var userId = Convert.ToInt32(Request.HttpContext.User.Claims.Where(a => a.Type == ClaimTypes.Sid).Select(a => a.Value).SingleOrDefault());

                var loginAuth = _loginAuth.SelectByID(userId);

                var today = DateTime.Today;
                var lastWeek = today.AddDays(7);

                var myCars = _myCars.SelectByFunc(a => a.UserId == userId && !a.IsDelete);
                myCars.ForEach(a =>
                {
                    a = _getCarDetail.CarDetail(a.Id);
                });

                var myDashboard = new DashboardVM()
                {
                    MyCars = myCars,
                    MyLocations = _locations.SelectByFunc(a => a.UserId == userId && !a.IsDelete),
                    User = _userDatas.SelectByID(userId),
                    Reservations = _reservations.SelectByFunc(a => a.UserId == userId && !a.IsDelete),
                    AWeekReservations = _reservations.SelectByFunc(a => a.ReservationDate >= today && a.ReservationDate < lastWeek && a.UserId == userId && !a.IsDelete)
                };

                return View(myDashboard);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
            
        }

        [HttpGet("panel/profile")]
        public IActionResult Profile()
        {
            var userId = Convert.ToInt32(Request.HttpContext.User.Claims.Where(a => a.Type == ClaimTypes.Sid).Select(a => a.Value).SingleOrDefault());

            var loginAuth = _loginAuth.SelectByID(userId);

            var user = _user.SelectByID(loginAuth.UserId);

            if (user == null) { return NotFound(); }
            user.LoginAuth = loginAuth;

            return View(user);
        }

        [HttpPost("panel/profile")]
        public IActionResult Profile(UserDatas updateUser)
        {
            try
            {
                var userId = Convert.ToInt32(Request.HttpContext.User.Claims.Where(a => a.Type == ClaimTypes.Sid).Select(a => a.Value).SingleOrDefault());

                var loginAuth = _loginAuth.SelectByID(userId);
                var user = _user.SelectByID(loginAuth.UserId);

                if (user == null) { return NotFound(); }

                user.Name = updateUser.Name;
                user.PhoneNumber = updateUser.PhoneNumber;
                user.Profession = updateUser.Profession;

                _user.Update(user);


                return Json(new { result = 1 });
            }
            catch (Exception)
            {
                return Json(new { });
            }

        }

        [HttpGet("panel/settings")]
        public IActionResult Settings()
        {
            return View();
        }

        [HttpGet("panel/change-password")]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost("panel/change-password", Name = "panelChangePassword")]
        public IActionResult ChangePassword(string oldPassword, string newPassword)
        {
            try
            {
                var userId = Convert.ToInt32(Request.HttpContext.User.Claims.Where(a => a.Type == ClaimTypes.Sid).Select(a => a.Value).SingleOrDefault());

                var user = _loginAuth.SelectByID(userId);
                if (user == null) { return NotFound(); }

                if (GetMD5(oldPassword) == user.Password)
                {
                    user.Password = GetMD5(newPassword);
                    _loginAuth.Update(user);
                    ViewBag.Message = "success";

                    return View();
                }

                ViewBag.Message = "wrongPass";
                return View();
            }
            catch (Exception)
            {
                ViewBag.Message = "Error";
                return View();
            }

        }

        [HttpGet("panel/bank-account")]
        public IActionResult BankAccount()
        {
            return View();
        }

        [HttpGet("panel/agreements")]
        public IActionResult Agreement()
        {
            return View();
        }

        [Authorize(Roles = "2")]
        [HttpGet("panel/my-company")]
        public IActionResult Company()
        {
            var userId = Convert.ToInt32(Request.HttpContext.User.Claims.Where(a => a.Type == ClaimTypes.Sid).Select(a => a.Value).SingleOrDefault());
            var loginAuth = _loginAuth.SelectByID(userId);
            var user = _user.SelectByID(loginAuth.UserId);
            if (user == null) { return NotFound(); }
            user.LoginAuth = loginAuth;
            return View(user);
        }

        [Authorize(Roles = "2")]
        [HttpPost("panel/my-company")]
        public IActionResult Company(UserDatas updateUser)
        {
            try
            {
                var userId = Convert.ToInt32(Request.HttpContext.User.Claims.Where(a => a.Type == ClaimTypes.Sid).Select(a => a.Value).SingleOrDefault());

                var loginAuth = _loginAuth.SelectByID(userId);
                var user = _user.SelectByID(loginAuth.UserId);
                if (user == null) { return NotFound(); }

                user.AboutUs = updateUser.AboutUs;
                user.CompanyPhoneNumber = updateUser.CompanyPhoneNumber;
                user.CompanyEmail = updateUser.CompanyEmail;
                user.CompanyWebsite = updateUser.CompanyWebsite;
                user.Address = updateUser.Address;
                user.CompanyName = updateUser.CompanyName;
                user.Country = updateUser.Country;
                user.Facebook = updateUser.Facebook;
                user.Linkedin = updateUser.Linkedin;

                _user.Update(user);

                return Json(new { result = 1 });
            }
            catch (Exception)
            {
                return Json(new { });
            }

        }

        [HttpGet("user-management")]
        public IActionResult UserManagement()
        {

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
