using Airport.DBEntities.Entities;
using Airport.UI.Models.VM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System;
using System.Security.Claims;
using Airport.DBEntitiesDAL.Interfaces;
using System.Collections.Generic;
using Airport.UI.Models.Interface;

namespace Airport.UI.Controllers;

[Authorize(Roles ="3")]
public class DriverReservationController(IReservationsDAL reservationsDal, IDriversDAL driversDal, IGetCarDetail carDetailDal, ILocationCarsDAL locationCarsDal, IReservationPeopleDAL reservationPeopleDal, ILocationCarsFareDAL locationCarsFareDal, IUserDatasDAL userDatasDal, IServicesDAL servicesDal, IServiceItemsDAL serviceItemsDal, IServicePropertiesDAL servicePropertiesDal, IServiceCategoriesDAL serviceCategoriesDal, IReservationServicesTableDAL reservationServicesTableDal, ILoginAuthDAL loginAuthDal, IMyCarsDAL myCarsDal) : Controller
{

    [HttpGet("driver-reservations")]
    public IActionResult Index()
    {
        try
        {
            var userId = Convert.ToInt32(Request.HttpContext.User.Claims.Where(a => a.Type == ClaimTypes.Sid).Select(a => a.Value).SingleOrDefault());
            var loginAuth = loginAuthDal.SelectByID(userId);
            var myCars = myCarsDal.SelectByFunc(a=>a.DriverId == loginAuth.DriverId);

            var myReservations = reservationsDal.SelectByFunc(a => a.DriverId == loginAuth.DriverId);


            myReservations.ForEach(a =>
            {
                a.LocationCars = locationCarsDal.SelectByID(a.LocationCarId);
                a.LocationCars.Car = carDetailDal.CarDetail(a.LocationCars.CarId);
                a.LocationCars.LocationCarsFares = locationCarsFareDal.SelectByFunc(b => b.LocationCarId == a.LocationCarId);
            });

            var reservationVM = new ReservationsIndexVM()
            {
                Reservations = myReservations
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
        var getReservation = reservationsDal.SelectByFunc(a => a.Id == id).FirstOrDefault();
        if (getReservation is not null)
        {
            getReservation.Status = 4;
            reservationsDal.Update(getReservation);

            return Json(new { result = 1 });
        }
        return BadRequest();
    }

    public IActionResult GetReservationNote(int id)
    {
        var userId = Convert.ToInt32(Request.HttpContext.User.Claims.Where(a => a.Type == ClaimTypes.Sid).Select(a => a.Value).SingleOrDefault());
        var reservation = reservationsDal.SelectByFunc(a => a.Id == id).FirstOrDefault();
        if (reservation is not null)
        {
            return Json(new { result = 1, Data = reservation });
        }
        return BadRequest();
    }

    public IActionResult UpdateReservationStatus(Reservations reservation)
    {
        var userId = Convert.ToInt32(Request.HttpContext.User.Claims.Where(a => a.Type == ClaimTypes.Sid).Select(a => a.Value).SingleOrDefault());
        var getReservation = reservationsDal.SelectByFunc(a => a.Id == reservation.Id).FirstOrDefault();
        if (getReservation is not null)
        {
            var list = new List<int> { 1, 3 };
            if (!list.Contains(reservation.Status))
            {
                reservation.Status = 1;
            }

            getReservation.FinishComment = reservation.FinishComment;
            getReservation.Status = reservation.Status;
            reservationsDal.Update(getReservation);

            return Json(new { result = 1 });
        }
        return BadRequest();
    }

    [HttpGet("reservation-driver-detail/{id}")]
    public IActionResult ReservationDetail(int id)
    {
        try
        {
            var userId = Convert.ToInt32(Request.HttpContext.User.Claims.Where(a => a.Type == ClaimTypes.Sid).Select(a => a.Value).SingleOrDefault());
            var reservation = reservationsDal.SelectByFunc(a => a.Id == id).FirstOrDefault();
            if (reservation is not null)
            {
                reservation.LocationCars = locationCarsDal.SelectByID(reservation.LocationCarId);
                reservation.LocationCars.Car = carDetailDal.CarDetail(reservation.LocationCars.CarId);
                reservation.LocationCars.Car.Service = servicesDal.SelectByID(reservation.LocationCars.Car.SeriesId);



                reservation.Driver = driversDal.SelectByID(reservation.DriverId);
                if (reservation.Driver != null)
                {
                    var loginAuth = loginAuthDal.SelectByFunc(a => a.DriverId == reservation.DriverId).FirstOrDefault();
                    reservation.Driver.LoginAuth = loginAuth;
                }
                reservation.ReservationPeoples = reservationPeopleDal.SelectByFunc(a => a.ReservationId == reservation.Id);

                var ReservationServicesTable = reservationServicesTableDal.SelectByFunc(a => a.ReservationId == id);
                ReservationServicesTable.ForEach(a =>
                {
                    a.ServiceItem = serviceItemsDal.SelectByID(a.ServiceItemId);
                    a.ServiceItem.ServiceProperty = servicePropertiesDal.SelectByID(a.ServiceItem.ServicePropertyId);
                    a.ServiceItem.Service = servicesDal.SelectByID(a.ServiceItem.ServiceId);
                    a.ServiceItem.ServiceProperty.ServiceCategory = serviceCategoriesDal.SelectByID(a.ServiceItem.ServiceProperty.ServiceCategoryId);
                });


                var reservationVM = new ReservationManagementVM()
                {
                    Reservation = reservation,
                    ReservationServicesTable = ReservationServicesTable
                };


                return View(reservationVM);
            }
            return NotFound();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.ToString());
        }
    }
}
