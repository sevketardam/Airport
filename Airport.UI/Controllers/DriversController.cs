using Airport.DBEntities.Entities;
using Airport.DBEntitiesDAL.Interfaces;
using Airport.UI.Models.IM;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System;
using System.Security.Cryptography;
using System.Security.Claims;

namespace Airport.UI.Controllers
{
    public class DriversController : PanelAuthController
    {
        IDriversDAL _drivers;
        IMyCarsDAL _myCars;
        IReservationsDAL _reservations;
        ILoginAuthDAL _loginAuth;
        public DriversController(IDriversDAL drivers, IMyCarsDAL myCars, IReservationsDAL reservations, ILoginAuthDAL loginAuth)
        {
            _drivers = drivers;
            _myCars = myCars;
            _reservations = reservations;
            _loginAuth = loginAuth;
        }

        [HttpGet("panel/my-drivers")]
        public IActionResult Index()
        {
            var userId = Convert.ToInt32(Request.HttpContext.User.Claims.Where(a => a.Type == ClaimTypes.Sid).Select(a => a.Value).SingleOrDefault());
            var myDrivers = _drivers.SelectByFunc(a => a.UserId == userId && !a.IsDelete);

            return View(myDrivers);
        }

        [HttpGet("panel/add-driver")]
        public IActionResult AddDriverPage()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddDriver(AddDriverIM newDriver)
        {
            try
            {
                var datas = _loginAuth.SelectByFunc(a => a.Email == newDriver.Email).FirstOrDefault();
                if (datas == null)
                {
                    var userId = Convert.ToInt32(Request.HttpContext.User.Claims.Where(a => a.Type == ClaimTypes.Sid).Select(a => a.Value).SingleOrDefault());

                    if (newDriver == null) { return BadRequest(); }

                    var driver = new Drivers()
                    {
                        Financial = newDriver.Financial,
                        Booking = newDriver.Booking,
                        Name = newDriver.Name,
                        Phone = newDriver.Phone,
                        UserId = userId,
                        DateOfBirth = newDriver.DateOfBirth,
                        PhotoFront = newDriver.PhotoFront,
                        PhotoBack = newDriver.PhotoBack,
                        Surname = newDriver.Surname,
                        DriverId = newDriver.DriverId,
                        IsDelete = false
                    };

                    var addedDriver = _drivers.Insert(driver);

                    _loginAuth.Insert(new LoginAuth
                    {
                        Email = newDriver.Email,
                        Password = GetMD5(newDriver.Password),
                        DriverId = addedDriver.Id,
                        UserId = 0,
                        Type = 3
                    });

                    return Json(new { result = 1 });
                }

                return Json(new { result = 2 });
            }
            catch (System.Exception)
            {
                return BadRequest();
            }
        }

        [HttpGet("panel/update-driver/{id}")]
        public IActionResult UpdateDriverPage(int id)
        {
            try
            {
                var userId = Convert.ToInt32(Request.HttpContext.User.Claims.Where(a => a.Type == ClaimTypes.Sid).Select(a => a.Value).SingleOrDefault());
                var myDriver = _drivers.SelectByFunc(a => a.Id == id && a.UserId == userId && !a.IsDelete).FirstOrDefault();
                if (myDriver != null)
                {
                    myDriver.LoginAuth = _loginAuth.SelectByFunc(a => a.DriverId == id).FirstOrDefault();
                    return View(myDriver);
                }
                return BadRequest();
            }
            catch (System.Exception)
            {

                return BadRequest();
            }
        }

        [HttpPost]
        public IActionResult UpdateDriver(UpdateDriverIM updatedValue, int id)
        {
            try
            {
                var checkEmail = _loginAuth.SelectByFunc(a => a.Email == updatedValue.Email && a.DriverId != id).FirstOrDefault();
                if (checkEmail is null)
                {
                    var userId = Convert.ToInt32(Request.HttpContext.User.Claims.Where(a => a.Type == ClaimTypes.Sid).Select(a => a.Value).SingleOrDefault());
                    var myDriver = _drivers.SelectByFunc(a => a.Id == id && a.UserId == userId && !a.IsDelete).FirstOrDefault();
                    if (myDriver != null)
                    {
                        myDriver.Financial = updatedValue.Financial;
                        myDriver.Booking = updatedValue.Booking;
                        myDriver.Name = updatedValue.Name;
                        myDriver.Phone = updatedValue.Phone;
                        myDriver.PhotoBack = updatedValue.PhotoBack;
                        myDriver.PhotoFront = updatedValue.PhotoFront;
                        myDriver.DriverId = updatedValue.DriverId;
                        myDriver.Surname = updatedValue.Surname;
                        myDriver.DateOfBirth = updatedValue.DateOfBirth;

                        _drivers.Update(myDriver);

                        var login = _loginAuth.SelectByFunc(a => a.DriverId == myDriver.Id).FirstOrDefault();
                        login.Email = updatedValue.Email;
                        _loginAuth.Update(login);

                        return Json(new { result = 1 });
                    }
                }
                return Json(new { result = 2 });
            }
            catch (System.Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        public IActionResult DeleteDriver(int id)
        {
            try
            {
                var userId = Convert.ToInt32(Request.HttpContext.User.Claims.Where(a => a.Type == ClaimTypes.Sid).Select(a => a.Value).SingleOrDefault());
                var myDriver = _drivers.SelectByFunc(a => a.Id == id && a.UserId == userId && !a.IsDelete).FirstOrDefault();
                if (myDriver != null)
                {
                    var myCars = _myCars.SelectByFunc(a => a.DriverId == myDriver.Id && !a.IsDelete);
                    myCars.ForEach(a =>
                    {
                        a.DriverId = null;
                        _myCars.Update(a);
                    });

                    var reservations = _reservations.SelectByFunc(a => a.DriverId == myDriver.Id);

                    if (reservations.Any())
                    {
                        _drivers.SoftDelete(myDriver);
                    }
                    else
                    {
                        _drivers.HardDelete(myDriver);
                    }

                    return Json(new { result = 1 });
                }
                return Json(new { result = 2 });
            }
            catch (System.Exception)
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
}
