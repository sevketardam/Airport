using Airport.DBEntities.Entities;
using Airport.UI.Models.VM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System;
using System.Security.Claims;
using Airport.DBEntitiesDAL.Interfaces;
using System.Collections.Generic;

namespace Airport.UI.Controllers
{
    [Authorize(Roles ="3")]
    public class DriverReservationController : Controller
    {
        IReservationsDAL _reservations;
        IDriversDAL _drivers;
        IMyCarsDAL _myCars;
        ILocationCarsDAL _locationCars;
        ILoginAuthDAL _loginAuth;
        public DriverReservationController(IReservationsDAL reservations, IDriversDAL drivers,IMyCarsDAL myCars, ILocationCarsDAL locationCars, ILoginAuthDAL loginAuth)
        {
            _reservations = reservations;
            _drivers = drivers;
            _myCars = myCars;
            _locationCars = locationCars;
            _loginAuth = loginAuth;

        }

        [HttpGet("driver-reservations")]
        public IActionResult Index()
        {
            try
            {
                var userId = Convert.ToInt32(Request.HttpContext.User.Claims.Where(a => a.Type == ClaimTypes.Sid).Select(a => a.Value).SingleOrDefault());
                var loginAuth = _loginAuth.SelectByID(userId);
                var myCars = _myCars.SelectByFunc(a=>a.DriverId == loginAuth.DriverId);
                var reservations = new List<Reservations>();
                myCars.ForEach(a =>
                {
                    a.LocationCars = _locationCars.SelectByFunc(b=>b.CarId == a.Id);
                    a.LocationCars.ForEach(b =>
                    {
                        b.Reservations = _reservations.SelectByFunc(c => c.LocationCarId == b.Id);
                        if (b.Reservations != null && b.Reservations.Count > 0)
                        {
                            reservations.AddRange(b.Reservations);
                        }
                    });
                });

                var reservationVM = new ReservationsIndexVM()
                {
                    Reservations = reservations
                };

                return View(reservationVM);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.ToString());
            }
        }

        public IActionResult CancelReservation(int id)
        {
            var userId = Convert.ToInt32(Request.HttpContext.User.Claims.Where(a => a.Type == ClaimTypes.Sid).Select(a => a.Value).SingleOrDefault());
            var getReservation = _reservations.SelectByFunc(a => a.Id == id).FirstOrDefault();
            if (getReservation is not null)
            {
                getReservation.Status = 4;
                _reservations.Update(getReservation);

                return Json(new { result = 1 });
            }
            return BadRequest();
        }

        public IActionResult GetReservationNote(int id)
        {
            var userId = Convert.ToInt32(Request.HttpContext.User.Claims.Where(a => a.Type == ClaimTypes.Sid).Select(a => a.Value).SingleOrDefault());
            var reservation = _reservations.SelectByFunc(a => a.Id == id).FirstOrDefault();
            if (reservation is not null)
            {
                return Json(new { result = 1, Data = reservation });
            }
            return BadRequest();
        }

        public IActionResult UpdateReservationStatus(Reservations reservation)
        {
            var userId = Convert.ToInt32(Request.HttpContext.User.Claims.Where(a => a.Type == ClaimTypes.Sid).Select(a => a.Value).SingleOrDefault());
            var getReservation = _reservations.SelectByFunc(a => a.Id == reservation.Id).FirstOrDefault();
            if (getReservation is not null)
            {
                var list = new List<int> { 1, 3 };
                if (!list.Contains(reservation.Status))
                {
                    reservation.Status = 1;
                }

                getReservation.FinishComment = reservation.FinishComment;
                getReservation.Status = reservation.Status;
                _reservations.Update(getReservation);

                return Json(new { result = 1 });
            }
            return BadRequest();
        }
    }
}
