using Airport.DBEntitiesDAL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System;
using System.Security.Claims;
using Airport.DBEntities.Entities;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authorization;

namespace Airport.UI.Controllers;

public class WebProfileController(IUserDatasDAL userDal, ILoginAuthDAL loginAuthDal) : BaseController
{
    [HttpGet("profile")]
    public IActionResult Index()
    {
        var userId = Convert.ToInt32(Request.HttpContext.User.Claims.Where(a => a.Type == ClaimTypes.Sid).Select(a => a.Value).SingleOrDefault());

        var loginAuth = loginAuthDal.SelectByID(userId);

        var user = userDal.SelectByID(loginAuth.UserId);

        if (user == null) { return NotFound(); }
        user.LoginAuth = loginAuth;

        return View(user);
    }

    [HttpPost("profile")]
    public IActionResult Index(UserDatas updateUser)
    {
        try
        {
            var userId = Convert.ToInt32(Request.HttpContext.User.Claims.Where(a => a.Type == ClaimTypes.Sid).Select(a => a.Value).SingleOrDefault());

            var loginAuth = loginAuthDal.SelectByID(userId);
            var user = userDal.SelectByID(loginAuth.UserId);
            
            if (user == null) { return NotFound(); }

            user.Name = updateUser.Name;
            user.PhoneNumber = updateUser.PhoneNumber;
            user.Profession = updateUser.Profession;
            user.Img = updateUser.Img;

            userDal.Update(user);


            return Json(new {result = 1});
        }
        catch (Exception)
        {
            return Json(new { });
        }

    }

    [HttpGet("settings")]
    public IActionResult Settings()
    {
        return View();
    }

    [HttpGet("change-password")]
    public IActionResult ChangePassword()
    {
        return View();
    }

    [HttpPost("change-password",Name ="changePassword")]
    public IActionResult ChangePassword(string oldPassword,string newPassword)
    {
        try
        {
            var userId = Convert.ToInt32(Request.HttpContext.User.Claims.Where(a => a.Type == ClaimTypes.Sid).Select(a => a.Value).SingleOrDefault());

            var user = loginAuthDal.SelectByID(userId);
            if (user == null)
            {
                return Json(new { });
            }

            if (GetMD5(oldPassword) == user.Password)
            {
                user.Password = GetMD5(newPassword);
                loginAuthDal.Update(user);
                //okey
                return Json(new { result = 1 });
            }

            //wrong password
            return Json(new { result = 2 });
        }
        catch (Exception ex)
        {

            //error
            return Json(new {});
        }

    }

    [HttpGet("bank-account")]
    public IActionResult BankAccount()
    {
        return View();
    }

    [HttpGet("agreements")]
    public IActionResult Agreement()
    {
        return View();
    }

    [Authorize(Roles ="2")]
    [HttpGet("my-company")]
    public IActionResult Company()
    {
        var userId = Convert.ToInt32(Request.HttpContext.User.Claims.Where(a => a.Type == ClaimTypes.Sid).Select(a => a.Value).SingleOrDefault());
        var loginAuth = loginAuthDal.SelectByID(userId);
        var user = userDal.SelectByID(loginAuth.UserId);
        if (user == null) { return NotFound(); }
        user.LoginAuth = loginAuth;
        return View(user);
    }

    [Authorize(Roles = "2")]
    [HttpPost("my-company")]
    public IActionResult Company(UserDatas updateUser)
    {
        try
        {
            var userId = Convert.ToInt32(Request.HttpContext.User.Claims.Where(a => a.Type == ClaimTypes.Sid).Select(a => a.Value).SingleOrDefault());

            var loginAuth = loginAuthDal.SelectByID(userId);
            var user = userDal.SelectByID(loginAuth.UserId);
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

            userDal.Update(user);

            return Json(new { result = 1 });
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
}
