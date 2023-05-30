using Airport.DBEntities.Entities;
using Airport.UI.Models.VM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System;
using System.Security.Claims;
using Airport.DBEntitiesDAL.Interfaces;
using System.Collections.Generic;
using Airport.MessageExtensions.Interfaces;
using Airport.UI.Models.Interface;
using Microsoft.AspNetCore.Hosting;

namespace Airport.UI.Controllers
{
    [Authorize(Roles ="3")]
    public class DriverReservationController : Controller
    {
        IReservationsDAL _reservations;
        IDriversDAL _drivers;
        IGetCarDetail _carDetail;
        ILocationCarsDAL _locationCars;
        IReservationPeopleDAL _reservationPeople;
        ILocationsDAL _location;
        ILocationCarsFareDAL _locationCarsFare;
        IUserDatasDAL _userDatas;
        IServicesDAL _services;
        IServiceItemsDAL _serviceItems;
        IServicePropertiesDAL _serviceProperties;
        IServiceCategoriesDAL _serviceCategories;
        IReservationServicesTableDAL _reservationServicesTable;
        ILoginAuthDAL _loginAuth;
        IMyCarsDAL _myCars;
        public DriverReservationController(IReservationsDAL reservations, IDriversDAL drivers, IGetCarDetail carDetail, ILocationCarsDAL locationCars, IReservationPeopleDAL reservationPeople, ILocationsDAL locations, ILocationCarsFareDAL locationCarsFare, IUserDatasDAL userDatas, IServicesDAL services, IServiceItemsDAL serviceItems, IServicePropertiesDAL serviceProperties, IServiceCategoriesDAL serviceCategories, IReservationServicesTableDAL reservationServicesTable, IWebHostEnvironment env, IMail mail, ILoginAuthDAL loginAuth, IMyCarsDAL myCars)
        {
            _drivers = drivers;
            _reservations = reservations;
            _carDetail = carDetail;
            _locationCars = locationCars;
            _reservationPeople = reservationPeople;
            _location = locations;
            _locationCarsFare = locationCarsFare;
            _userDatas = userDatas;
            _services = services;
            _serviceItems = serviceItems;
            _serviceProperties = serviceProperties;
            _serviceCategories = serviceCategories;
            _reservationServicesTable = reservationServicesTable;
            _loginAuth = loginAuth;
            _myCars = myCars;
        }

        [HttpGet("driver-reservations")]
        public IActionResult Index()
        {
            try
            {
                var userId = Convert.ToInt32(Request.HttpContext.User.Claims.Where(a => a.Type == ClaimTypes.Sid).Select(a => a.Value).SingleOrDefault());
                var loginAuth = _loginAuth.SelectByID(userId);
                var myCars = _myCars.SelectByFunc(a=>a.DriverId == loginAuth.DriverId);
                //var reservations = new List<Reservations>();

                var myReservations = _reservations.SelectByFunc(a => a.DriverId == loginAuth.DriverId);


                myReservations.ForEach(a =>
                {
                    a.LocationCars = _locationCars.SelectByID(a.LocationCarId);
                    a.LocationCars.Car = _carDetail.CarDetail(a.LocationCars.CarId);
                    a.LocationCars.LocationCarsFares = _locationCarsFare.SelectByFunc(b => b.LocationCarId == a.LocationCarId);
                });

                //myCars.ForEach(a =>
                //{
                //    a.LocationCars = _locationCars.SelectByFunc(b=>b.CarId == a.Id);
                //    a.LocationCars.ForEach(b =>
                //    {
                //        b.Reservations = _reservations.SelectByFunc(c => c.LocationCarId == b.Id);
                //        if (b.Reservations != null && b.Reservations.Count > 0)
                //        {
                //            reservations.AddRange(b.Reservations);
                //        }
                //    });
                //});

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

        [HttpGet("reservation-driver-detail/{id}")]
        public IActionResult ReservationDetail(int id)
        {
            try
            {
                var userId = Convert.ToInt32(Request.HttpContext.User.Claims.Where(a => a.Type == ClaimTypes.Sid).Select(a => a.Value).SingleOrDefault());
                var reservation = _reservations.SelectByFunc(a => a.Id == id).FirstOrDefault();
                if (reservation is not null)
                {
                    reservation.LocationCars = _locationCars.SelectByID(reservation.LocationCarId);
                    reservation.LocationCars.Car = _carDetail.CarDetail(reservation.LocationCars.CarId);
                    reservation.LocationCars.Car.Service = _services.SelectByID(reservation.LocationCars.Car.SeriesId);



                    reservation.Driver = _drivers.SelectByID(reservation.DriverId);
                    if (reservation.Driver != null)
                    {
                        var loginAuth = _loginAuth.SelectByFunc(a => a.DriverId == reservation.DriverId).FirstOrDefault();
                        reservation.Driver.LoginAuth = loginAuth;
                    }
                    reservation.ReservationPeoples = _reservationPeople.SelectByFunc(a => a.ReservationId == reservation.Id);

                    var ReservationServicesTable = _reservationServicesTable.SelectByFunc(a => a.ReservationId == id);
                    ReservationServicesTable.ForEach(a =>
                    {
                        a.ServiceItem = _serviceItems.SelectByID(a.ServiceItemId);
                        a.ServiceItem.ServiceProperty = _serviceProperties.SelectByID(a.ServiceItem.ServicePropertyId);
                        a.ServiceItem.Service = _services.SelectByID(a.ServiceItem.ServiceId);
                        a.ServiceItem.ServiceProperty.ServiceCategory = _serviceCategories.SelectByID(a.ServiceItem.ServiceProperty.ServiceCategoryId);
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
}
