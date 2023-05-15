using Airport.DBEntitiesDAL.Interfaces;
using Airport.UI.Models.VM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System;
using System.Security.Claims;
using Airport.UI.Models.Interface;

namespace Airport.UI.Controllers
{
    public class PanelController : PanelAuthController
    {
        IMyCarsDAL _myCars;
        IUserDatasDAL _userDatas;
        ILocationsDAL _locations;
        IGetCarDetail _getCarDetail;
        IReservationsDAL _reservations;
        public PanelController(IMyCarsDAL myCars, IUserDatasDAL userDatas, ILocationsDAL locations, IGetCarDetail getCarDetail,IReservationsDAL reservations)
        {
            _myCars = myCars;
            _userDatas = userDatas;
            _locations = locations;
            _getCarDetail = getCarDetail;
            _reservations = reservations;   
        }
        public IActionResult Index()
        {

            var userId = Convert.ToInt32(Request.HttpContext.User.Claims.Where(a => a.Type == ClaimTypes.Sid).Select(a => a.Value).SingleOrDefault());
            var today = DateTime.Today;
            var lastWeek = today.AddDays(7);

            var myCars = _myCars.SelectByFunc(a => a.UserId == userId);
            myCars.ForEach(a =>
            {
                a = _getCarDetail.CarDetail(a.Id);
            });

            var myDashboard = new DashboardVM()
            {
                MyCars = myCars,
                MyLocations = _locations.SelectByFunc(a => a.UserId == userId),
                User = _userDatas.SelectByID(userId),
                Reservations = _reservations.SelectByFunc(a => a.UserId == userId && !a.IsDelete),
                AWeekReservations = _reservations.SelectByFunc(a => a.ReservationDate >= today && a.ReservationDate < lastWeek && a.UserId == userId && !a.IsDelete)
            };

            return View(myDashboard);
        }
    }
}
