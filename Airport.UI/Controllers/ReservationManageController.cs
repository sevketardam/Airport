using Airport.DBEntitiesDAL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System;
using System.Security.Claims;
using Airport.UI.Models.VM;
using iText.Layout.Element;
using Airport.DBEntities.Entities;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace Airport.UI.Controllers
{
    public class ReservationManageController : Controller
    {
        IReservationsDAL _reservations;
        IDriversDAL _drivers;
        public ReservationManageController(IReservationsDAL reservations, IDriversDAL drivers)
        {
            _drivers = drivers;
            _reservations = reservations;
        }

        [HttpGet("panel/reservation-management")]
        public IActionResult Index()
        {
            var userId = Convert.ToInt32(Request.HttpContext.User.Claims.Where(a => a.Type == ClaimTypes.Sid).Select(a => a.Value).SingleOrDefault());
            var reservationVM = new ReservationsIndexVM()
            {
                Reservations = _reservations.SelectByFunc(a => a.UserId == userId),
                Drivers = _drivers.SelectByFunc(a => a.UserId == userId)
            };


            return View(reservationVM);
        }

        [HttpGet("panel/reservation-detail/{id}")]
        public IActionResult ReservationDetail(int id)
        {

            var userId = Convert.ToInt32(Request.HttpContext.User.Claims.Where(a => a.Type == ClaimTypes.Sid).Select(a => a.Value).SingleOrDefault());
            var reservation = _reservations.SelectByFunc(a => a.Id == id && a.UserId == userId).FirstOrDefault();
            if (reservation is not null)
            {


                var reservationVM = new ReservationManagementVM()
                {
                    Reservation = reservation,
                    Drivers = _drivers.SelectByFunc(a => a.UserId == userId)
                };


                return View(reservationVM);
            }
            return NotFound();
        }

        public IActionResult GetReservationNote(int id)
        {
            var userId = Convert.ToInt32(Request.HttpContext.User.Claims.Where(a => a.Type == ClaimTypes.Sid).Select(a => a.Value).SingleOrDefault());
            var reservation = _reservations.SelectByFunc(a => a.Id == id && a.UserId == userId).FirstOrDefault();
            if (reservation is not null)
            {
                return Json(new { result = 1, Data = reservation });
            }
            return BadRequest();
        }

        public IActionResult GetDriverDetail(int id)
        {
            try
            {
                var userId = Convert.ToInt32(Request.HttpContext.User.Claims.Where(a => a.Type == ClaimTypes.Sid).Select(a => a.Value).SingleOrDefault());
                var reservation = _reservations.SelectByFunc(a => a.Id == id && a.UserId == userId).FirstOrDefault();
                if (reservation is not null)
                {
                    return Json(new { result = 1, Data = reservation });
                }
                return Json(new { result = 1 });
            }
            catch (Exception)
            {
                return Json(new { });
            }

        }

        public IActionResult AssignDriver(int driverId,int reservationId)
        {
            try
            {
                var userId = Convert.ToInt32(Request.HttpContext.User.Claims.Where(a => a.Type == ClaimTypes.Sid).Select(a => a.Value).SingleOrDefault());
                var driver = _drivers.SelectByFunc(a => a.Id == driverId && a.UserId == userId).FirstOrDefault();
                var reservation = _reservations.SelectByFunc(a => a.Id == reservationId && a.UserId == userId).FirstOrDefault();
                if (driver is not null && reservation is not null)
                {
                    reservation.DriverId = driver.Id;
                    _reservations.Update(reservation);

                    return Json(new { result = 1 });
                }
                return Json(new { result = 2 });
            }
            catch (Exception)
            {
                return Json(new { });
            }

        }

        public IActionResult UpdateReservationStatus(Reservations reservation)
        {
            var userId = Convert.ToInt32(Request.HttpContext.User.Claims.Where(a => a.Type == ClaimTypes.Sid).Select(a => a.Value).SingleOrDefault());
            var getReservation = _reservations.SelectByFunc(a => a.Id == reservation.Id && a.UserId == userId).FirstOrDefault();
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
