using Airport.DBEntitiesDAL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System;
using System.Security.Claims;
using Airport.DBEntities.Entities;
using System.Security.Cryptography;

namespace Airport.UI.Controllers
{
    public class WebProfileController : BaseController
    {
        IUserDatasDAL _user;
        public WebProfileController(IUserDatasDAL user)
        {
            _user = user;
        }

        [HttpGet("profile")]
        public IActionResult Index()
        {
            var userId = Convert.ToInt32(Request.HttpContext.User.Claims.Where(a => a.Type == ClaimTypes.Sid).Select(a => a.Value).SingleOrDefault());

            var user = _user.SelectByID(userId);
            if (user == null) { return NotFound(); }

            return View(user);
        }

        [HttpPost("profile",Name = "updateUserDatas")]
        public IActionResult Index(UserDatas updateUser)
        {
            try
            {
                var userId = Convert.ToInt32(Request.HttpContext.User.Claims.Where(a => a.Type == ClaimTypes.Sid).Select(a => a.Value).SingleOrDefault());

                var user = _user.SelectByID(userId);
                if (user == null) { return NotFound(); }

                user.Name = updateUser.Name;
                user.PhoneNumber = updateUser.PhoneNumber;
                user.Profession = updateUser.Profession;

                _user.Update(user);

                ViewBag.Message = "success";

                return View(user);
            }
            catch (Exception)
            {
                return BadRequest();
            }

        }

        [HttpGet("profile/settings")]
        public IActionResult Settings()
        {
            return View();
        }

        [HttpGet("profile/change-password")]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost("profile/change-password",Name ="changePassword")]
        public IActionResult ChangePassword(string oldPassword,string newPassword)
        {
            try
            {
                var userId = Convert.ToInt32(Request.HttpContext.User.Claims.Where(a => a.Type == ClaimTypes.Sid).Select(a => a.Value).SingleOrDefault());

                var user = _user.SelectByID(userId);
                if (user == null) { return NotFound(); }

                if (GetMD5(oldPassword) == user.Password)
                {
                    user.Password = GetMD5(newPassword);
                    _user.Update(user);
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

        [HttpGet("profile/bank-account")]
        public IActionResult BankAccount()
        {
            return View();
        }

        [HttpGet("profile/agreements")]
        public IActionResult Agreement()
        {
            return View();
        }

        [HttpGet("profile/my-company")]
        public IActionResult Company()
        {
            var userId = Convert.ToInt32(Request.HttpContext.User.Claims.Where(a => a.Type == ClaimTypes.Sid).Select(a => a.Value).SingleOrDefault());

            var user = _user.SelectByID(userId);
            if (user == null) { return NotFound(); }

            return View(user);
        }

        [HttpPost("profile/my-company", Name = "updateCompanyDatas")]
        public IActionResult Company(UserDatas updateUser)
        {
            try
            {
                var userId = Convert.ToInt32(Request.HttpContext.User.Claims.Where(a => a.Type == ClaimTypes.Sid).Select(a => a.Value).SingleOrDefault());

                var user = _user.SelectByID(userId);
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

                ViewBag.Message = "success";
                return View(user);
            }
            catch (Exception)
            {
                ViewBag.Message = "Error";
                return RedirectToAction("Company","WebProfile");
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
    }
}
