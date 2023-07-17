﻿using Airport.DBEntitiesDAL.Interfaces;
using Airport.UI.Models.VM;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace Airport.UI.Controllers
{
    public class UsersController : PanelAuthController
    {
        IUserDatasDAL _userData;
        ILoginAuthDAL _loginAuth;
        IDriversDAL _driver;
        public UsersController(IUserDatasDAL userData, ILoginAuthDAL loginAuth, IDriversDAL driver)
        {
            _loginAuth = loginAuth;
            _userData = userData;
            _driver = driver;

        }

        [HttpGet("user-list")]
        public IActionResult Index()
        {
            var userList = new List<UserListVM>();
            //0=global admin,1=müşteri,2=partner acente,3=sürücü,4=junior admin,5=satış acentesi
            var authList = _loginAuth.Select();
            authList.ForEach(a =>
            {
                if (a.Type == 0 || a.Type == 1 || a.Type == 2 || a.Type == 4 || a.Type == 5)
                {
                    a.User = _userData.SelectByID(a.UserId);
                }
                else if (a.Type == 3)
                {
                    a.Driver = _driver.SelectByID(a.DriverId);
                    a.Driver.User = _userData.SelectByID(a.Driver.UserId);
                }

                userList.Add(new UserListVM
                {
                    CompanyName = a.Type == 3 ? a.Driver.User?.CompanyName : a.Type == 2 ? "-": a.User?.CompanyName,
                    Email = a.Email,
                    Name = a.Type == 3 ? a.Driver?.Name + " " + a.Driver?.Surname : a.User?.Name,
                    Role = a.Type,
                    Id = a.Id
                });

            });

            return View(userList);
        }
    }
}
