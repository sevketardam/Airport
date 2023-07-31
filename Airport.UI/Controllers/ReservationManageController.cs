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
using Airport.UI.Models.Interface;
using Airport.UI.Models.IM;
using Airport.UI.Models.Extendions;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using Airport.MessageExtensions.Interfaces;
using Microsoft.AspNetCore.Hosting;
using System.Data;
using System.IO;
using Microsoft.Extensions.Options;
using Airport.MessageExtension.Interfaces;
using Airport.MessageExtension.VM;
using Microsoft.Extensions.Configuration;

namespace Airport.UI.Controllers
{
    public class ReservationManageController : PanelAuthController
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        private readonly IOptions<GoogleAPIKeys> _googleAPIKeys;

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
        IMail _mail;
        ILoginAuthDAL _loginAuth;
        ICouponsDAL _coupons;
        ITReservations _reservationT;
        ISMS _sms;
        IApiResult _apiResult;
        ITReservationHelpers _tReservationHelpers;
        public ReservationManageController(IReservationsDAL reservations, ISMS sms, IDriversDAL drivers, IGetCarDetail carDetail, ILocationCarsDAL locationCars, IReservationPeopleDAL reservationPeople, ILocationsDAL locations, ILocationCarsFareDAL locationCarsFare, IUserDatasDAL userDatas, IServicesDAL services, IServiceItemsDAL serviceItems, IServicePropertiesDAL serviceProperties, IServiceCategoriesDAL serviceCategories, IReservationServicesTableDAL reservationServicesTable, IWebHostEnvironment env, IMail mail, ILoginAuthDAL loginAuth, ICouponsDAL coupons, ITReservations reservationT, IOptions<GoogleAPIKeys> googleAPIKeys, IApiResult apiResult, ITReservationHelpers tReservationHelpers,IConfiguration configuration)
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
            _env = env;
            _mail = mail;
            _loginAuth = loginAuth;
            _coupons = coupons;
            _reservationT = reservationT;
            _googleAPIKeys = googleAPIKeys;
            _sms = sms;
            _apiResult = apiResult;
            _tReservationHelpers = tReservationHelpers;
            _configuration = configuration;
        }


        [HttpGet("panel/reservation-management")]
        public IActionResult Index()
        {
            try
            {
                var userRole = User.Claims.Where(a => a.Type == ClaimTypes.Role).Select(a => a.Value).SingleOrDefault();

                var userId = Convert.ToInt32(Request.HttpContext.User.Claims.Where(a => a.Type == ClaimTypes.Sid).Select(a => a.Value).SingleOrDefault());
                var reservations = new List<Reservations>();
                var drivers = new List<Drivers>();
                if (userRole == "5")
                {
                    reservations = _reservations.SelectByFunc(a => a.SalesAgencyId == userId).OrderByDescending(a => a.ReservationDate).ToList();
                }
                else if (userRole == "0")
                {
                    reservations = _reservations.Select().OrderByDescending(a => a.ReservationDate).ToList();
                    drivers = _drivers.SelectByFunc(a => a.UserId == userId && !a.IsDelete);
                    reservations.ForEach(a =>
                    {
                        a.LocationCars = _locationCars.SelectByID(a.LocationCarId);
                        a.LocationCars.Location = _location.SelectByID(a.LocationCars?.LocationId);
                        var userAuth = _loginAuth.SelectByID(a.LocationCars?.Location?.UserId);
                        a.LocationCars.Location.User = _userDatas.SelectByID(userAuth?.UserId);
                    });
                }
                else
                {
                    reservations = _reservations.SelectByFunc(a => a.UserId == userId).OrderByDescending(a => a.ReservationDate).ToList();
                    drivers = _drivers.SelectByFunc(a => a.UserId == userId && !a.IsDelete);
                }

                var reservationVM = new ReservationsIndexVM()
                {
                    Reservations = reservations,
                    Drivers = drivers,
                };

                return View(reservationVM);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.ToString());
            }
        }

        [HttpGet("panel/reservation-detail/{id}")]
        public IActionResult ReservationDetail(int id)
        {
            try
            {
                var userId = Convert.ToInt32(Request.HttpContext.User.Claims.Where(a => a.Type == ClaimTypes.Sid).Select(a => a.Value).SingleOrDefault());

                var reservation = _reservations.SelectByFunc(a => a.Id == id).FirstOrDefault();
                if (reservation is not null)
                {
                    var reservationLocationCars = _locationCars.SelectByID(reservation.LocationCarId);

                    reservation.LocationCars = reservationLocationCars;
                    if (reservationLocationCars != null)
                    {
                        reservation.LocationCars.Car = _carDetail.CarDetail(reservation.LocationCars.CarId);
                        reservation.LocationCars.Car.Service = _services.SelectByID(reservation.LocationCars?.Car?.SeriesId);


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
                            a.ServiceItem.ServiceProperty = _serviceProperties.SelectByID(a.ServiceItem?.ServicePropertyId);
                            a.ServiceItem.Service = _services.SelectByID(a.ServiceItem?.ServiceId);
                            a.ServiceItem.ServiceProperty.ServiceCategory = _serviceCategories.SelectByID(a.ServiceItem?.ServiceProperty?.ServiceCategoryId);
                        });

                        if (reservation.Coupon != 0 && reservation.Coupon != null)
                        {
                            reservation.Coupons = _coupons.SelectByID(reservation.Coupon);
                        }

                        var reservationVM = new ReservationManagementVM()
                        {
                            Reservation = reservation,
                            Drivers = _drivers.SelectByFunc(a => a.UserId == userId && !a.IsDelete),
                            ReservationServicesTable = ReservationServicesTable,
                        };
                        return View(reservationVM);
                    }
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                string dosyaYolu = "wwwroot/error.txt";

                using (StreamWriter yazici = new StreamWriter(dosyaYolu))
                {
                    string metin = ex.ToString();
                    yazici.WriteLine(metin);
                }

                return RedirectToAction("Index", "Home");
            }
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

        public IActionResult AssignDriver(int driverId, int reservationId)
        {
            try
            {
                var userId = Convert.ToInt32(Request.HttpContext.User.Claims.Where(a => a.Type == ClaimTypes.Sid).Select(a => a.Value).SingleOrDefault());
                var driver = _drivers.SelectByFunc(a => a.Id == driverId && a.UserId == userId && !a.IsDelete).FirstOrDefault();
                var reservation = _reservations.SelectByFunc(a => a.Id == reservationId && a.UserId == userId).FirstOrDefault();
                if (driver is not null && reservation is not null)
                {
                    if (!(reservation.Status == 3 && reservation.LastUpdate.AddDays(1) <= DateTime.Now))
                    {
                        reservation.DriverId = driver.Id;
                        reservation.IsManuelDriver = false;
                        _reservations.Update(reservation);
                        return Json(new { result = 1 });
                    }
                    return Json(new { result = 3 });
                }
                return Json(new { result = 2 });
            }
            catch (Exception)
            {
                return Json(new { });
            }
        }

        public IActionResult GetDriverData(int driverId)
        {
            try
            {
                var userId = Convert.ToInt32(Request.HttpContext.User.Claims.Where(a => a.Type == ClaimTypes.Sid).Select(a => a.Value).SingleOrDefault());
                var driver = _drivers.SelectByFunc(a => a.Id == driverId && a.UserId == userId && !a.IsDelete).FirstOrDefault();
                if (driver is not null)
                {
                    var newDriver = new GetDriverVM()
                    {
                        DriverBirthday = driver.DateOfBirth.ToString("d"),
                        DriverId = driver.DriverId,
                        DriverPhone = driver.Phone,
                        DriverName = driver.Name,
                        DriverSurname = driver.Surname,
                        DriverEmail = _loginAuth.SelectByFunc(a => a.DriverId == driverId).FirstOrDefault().Email,
                        DriverBackPhoto = driver.PhotoBack,
                        DriverFrontPhoto = driver.PhotoFront,
                    };

                    return Json(new { result = 1, data = newDriver });
                }
                return Json(new { result = 2 });
            }
            catch (Exception)
            {
                return Json(new { });
            }
        }


        public IActionResult AddManualDriver(int reservationId, AddManualDriverIM driver)
        {
            try
            {
                var userId = Convert.ToInt32(Request.HttpContext.User.Claims.Where(a => a.Type == ClaimTypes.Sid).Select(a => a.Value).SingleOrDefault());
                var reservation = _reservations.SelectByFunc(a => a.Id == reservationId && a.UserId == userId).FirstOrDefault();
                if (reservation is not null)
                {
                    if (!(reservation.Status == 3 && reservation.LastUpdate.AddDays(1) <= DateTime.Now))
                    {
                        reservation.IsManuelDriver = true;
                        reservation.DriverName = driver.DriverName;
                        reservation.DriverSurname = driver.DriverSurname;
                        reservation.DriverPhone = driver.DriverPhone;
                        reservation.DriverFee = driver.DriverFee;
                        reservation.ManuelPlate = driver.ManuelPlate;
                        _reservations.Update(reservation);

                        var allMessage = new List<Mesaj>();
                        allMessage.Add(new Mesaj
                        {
                            dest = reservation.RealPhone,
                            msg = @$"Your driver is ready. Voucher Link {_configuration["PageLinks:PageGlobalLink"]}/pdf/{reservation.ReservationCode}-{reservation.Id}.pdf"
                        });

                        var mesaj = allMessage.ToArray();
                        _sms.SmsForReservation(mesaj);

                        return Json(new { result = 1 });
                    }
                    return Json(new { result = 3 });
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
                if (!(getReservation.Status == 3 && getReservation.LastUpdate.AddDays(1) <= DateTime.Now))
                {
                    var list = new List<int> { 1, 3 };
                    if (!list.Contains(reservation.Status))
                    {
                        reservation.Status = 1;
                    }

                    getReservation.FinishComment = reservation.FinishComment;
                    getReservation.Status = reservation.Status;
                    getReservation.LastUpdate = DateTime.Now;

                    _reservations.Update(getReservation);

                    return Json(new { result = 1 });
                }
                return Json(new { result = 2 });
            }
            return Json(new { result = 3 });
        }

        public IActionResult CancelReservation(int id)
        {
            var userId = Convert.ToInt32(Request.HttpContext.User.Claims.Where(a => a.Type == ClaimTypes.Sid).Select(a => a.Value).SingleOrDefault());
            var getReservation = _reservations.SelectByFunc(a => a.Id == id && a.UserId == userId).FirstOrDefault();
            if (getReservation is not null)
            {
                if (!(getReservation.Status == 3 && getReservation.LastUpdate.AddDays(1) <= DateTime.Now))
                {
                    getReservation.Status = 4;
                    _reservations.Update(getReservation);

                    return Json(new { result = 1 });
                }

                return Json(new { result = 2 });
            }

            return Json(new { });
        }

        [HttpGet("panel/update-reservation/{id}")]
        public IActionResult UpdateReservationStepOne(int id)
        {
            try
            {
                var userId = Convert.ToInt32(Request.HttpContext.User.Claims.Where(a => a.Type == ClaimTypes.Sid).Select(a => a.Value).SingleOrDefault());

                var userRole = User.Claims.Where(a => a.Type == ClaimTypes.Role).Select(a => a.Value).SingleOrDefault();
                Func<Reservations, bool> isAdmin = a => a.Id == id && a.UserId == userId;

                if (userRole == "0" || userRole == "4")
                {
                    isAdmin = a => a.Id == id;
                }

                var reservation = _reservations.SelectByFunc(isAdmin).FirstOrDefault();
                if (!(reservation.Status == 3 && reservation.LastUpdate.AddDays(1) <= DateTime.Now))
                {
                    if (reservation != null)
                    {
                        return View(reservation);
                    }
                }
                return NotFound();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost("panel/update-reservation/{id}", Name = "UpdateStepTwo")]
        public async Task<IActionResult> UpdateReservationStepTwo(GetResevationIM reservation, int id)
        {
            try
            {
                var selectedReservation = _reservations.SelectByID(id);
                if (selectedReservation is not null)
                {
                    var userId = Convert.ToInt32(Request.HttpContext.User.Claims.Where(a => a.Type == ClaimTypes.Sid).Select(a => a.Value).SingleOrDefault());

                    var pickLocationValues = await _apiResult.LocationValues(reservation.PickValue);

                    var dropLocationValues = await _apiResult.LocationValues(reservation.DropValue);

                    var betweenLocation = await _apiResult.DistanceMatrixValues(pickLocationValues.Result.Geometry.Location.lat, pickLocationValues.Result.Geometry.Location.lng, dropLocationValues.Result.Geometry.Location.lat, dropLocationValues.Result.Geometry.Location.lng);

                    if (betweenLocation.status == "OK")
                    {
                        var allDatas = await _reservationT.GetLocationAllDataList(pickLocationValues, dropLocationValues);

                        var getreservation = new List<GetReservationValues>();
                        var selectedLocationsMini = new List<LocationIsOutMiniVM>();

                        var selectedLocations = await _reservationT.GetLocationIsOutList(allDatas);

                        double minKm = 0;

                        if (betweenLocation.rows[0].elements[0].status == "OK")
                        {
                            var lastKm = Math.Round(Convert.ToDouble(betweenLocation.rows[0].elements[0].distance.value) / 100) * 100;
                            minKm = lastKm / 1000;
                            var lastLocations = await _reservationT.ReservationList(selectedLocations, reservation, minKm, pickLocationValues, dropLocationValues);

                            getreservation = lastLocations.Locations;
                            selectedLocationsMini = lastLocations.MiniLocations;
                        }


                        var updateReservation = _reservations.SelectByID(id);


                        var lastVM = new ReservationStepTwoVM()
                        {
                            ReservationValues = getreservation,
                            DropLocationLatLng = $"lat:{pickLocationValues.Result.Geometry.Location.lat},lng:{pickLocationValues.Result.Geometry.Location.lng}",
                            PickLocationLatLng = $"lat:{dropLocationValues.Result.Geometry.Location.lat},lng:{dropLocationValues.Result.Geometry.Location.lng}",
                            DropLocationPlaceId = reservation.DropValue,
                            PickLocationPlaceId = reservation.PickValue,
                            Distance = betweenLocation.rows[0].elements[0].distance.text,
                            Duration = betweenLocation.rows[0].elements[0].duration.text,
                            SelectedReservationValues = reservation,
                            UpdateReservationValues = updateReservation
                        };

                        var reservationDatas = new ReservationDatasVM()
                        {
                            DropLocationLatLng = $"lat:{pickLocationValues.Result.Geometry.Location.lat},lng:{pickLocationValues.Result.Geometry.Location.lng}",
                            PickLocationLatLng = $"lat:{dropLocationValues.Result.Geometry.Location.lat},lng:{dropLocationValues.Result.Geometry.Location.lng}",
                            DropLocationPlaceId = reservation.DropValue,
                            PickLocationPlaceId = reservation.PickValue,
                            PickLocationName = pickLocationValues.Result.formatted_address,
                            DropLocationName = dropLocationValues.Result.formatted_address,
                            KM = minKm,
                            ReservationValues = reservation,
                            Distance = betweenLocation.rows[0].elements[0].distance.text,
                            Duration = betweenLocation.rows[0].elements[0].duration.text,
                            UpdateReservation = updateReservation
                        };



                        HttpContext.Session.Remove("reservationData");
                        HttpContext.Session.MySet("reservationData", reservationDatas);
                        HttpContext.Session.MySet("selectedLocationMini", selectedLocationsMini);

                        lastVM.ReservationValues = lastVM.ReservationValues.OrderBy(a => a.LastPrice).ToList();

                        return View(lastVM);
                    }
                }


                return RedirectToAction("Index", "Home");
            }
            catch (Exception)
            {
                ViewBag.Error = "Error";
                return RedirectToAction("Index", "Home");
            }

        }

        [HttpGet("panel/update-reservation-user/{id}")]
        public IActionResult UpdateReservationStepThree(int id)
        {
            try
            {
                var selectedReservation = _reservations.SelectByID(id);
                if (selectedReservation is not null)
                {
                    var userId = Convert.ToInt32(Request.HttpContext.User.Claims.Where(a => a.Type == ClaimTypes.Sid).Select(a => a.Value).SingleOrDefault());
                    var user = new UserDatas();

                    if (userId != 0)
                    {
                        var loginAuth = _loginAuth.SelectByID(userId);
                        user = _userDatas.SelectByID(loginAuth?.UserId);
                        user.LoginAuth = loginAuth;
                    }
                    else
                    {
                        user = null;
                    }

                    var datas = HttpContext.Session.MyGet<ReservationDatasVM>("reservationData");

                    if (datas == null)
                    {
                        return NotFound();
                    }

                    var selectedDatasMini = HttpContext.Session.MyGet<List<LocationIsOutMiniVM>>("selectedLocationMini").Where(a => a.LocationCarId == id).FirstOrDefault();

                    if (selectedDatasMini != null)
                    {
                        var locationCar = _locationCars.SelectByID(selectedDatasMini.LocationCarId);

                        var prices = _tReservationHelpers.ReservationPrice(locationCar.Id, datas.KM, false, 0, datas.ReservationValues.ReturnStatus, selectedDatasMini.IsOutZone);

                        datas.TotalPrice = prices.LastPrice;
                        datas.LocationCar = locationCar;
                        datas.LocationCar.Car = _carDetail.CarDetail(locationCar.CarId);
                        datas.LocationCar.Car.Service = _services.SelectByID(datas.LocationCar.Car.ServiceId);
                        datas.IsOutZone = selectedDatasMini.IsOutZone;
                        var selectedCarItems = new List<PriceServiceList>();
                        if (datas.LocationCar.Car.Service != null)
                        {
                            datas.LocationCar.Car.Service.ServiceItems = _serviceItems.SelectByFunc(a => a.ServiceId == datas.LocationCar.Car.ServiceId);

                            var listServices = new List<ServiceCategories>();

                            var c = new List<PriceService>();

                            datas.LocationCar.Car.Service.ServiceItems.ForEach(a =>
                            {
                                a.ServiceProperty = _serviceProperties.SelectByID(a.ServicePropertyId);
                                a.ServiceProperty.ServiceCategory = _serviceCategories.SelectByID(a.ServiceProperty.ServiceCategoryId);
                                c.Add(new PriceService
                                {
                                    Category = a.ServiceProperty.ServiceCategory,
                                    CategoryProp = a.ServiceProperty
                                });
                            });

                            selectedCarItems = c.GroupBy(c => c.Category)
                                        .Select(g => new PriceServiceList
                                        {
                                            Category = g.Key,
                                            CategoryProps = g.Select(c => c.CategoryProp).ToList(),
                                        }).ToList();
                        }

                        HttpContext.Session.Remove("reservationData");

                        HttpContext.Session.MySet("reservationData", datas);

                        var updateServices = new List<ServiceItems>();
                        datas.UpdateReservation.ReservationServicesTables = _reservationServicesTable.SelectByFunc(a => a.ReservationId == datas.UpdateReservation.Id);
                        datas.UpdateReservation.ReservationServicesTables.ForEach(a =>
                        {
                            updateServices.Add(_serviceItems.SelectByID(a.ServiceItemId));
                        });


                        var reservation = new ReservationStepThreeVM()
                        {
                            SelectedData = datas,
                            User = user,
                            ServiceItems = selectedCarItems,
                            UpdateReservation = datas.UpdateReservation,
                            UpdateServiceItem = updateServices
                        };

                        return View(reservation);
                    }
                }
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error";
                return BadRequest(ex.ToString());
            }

        }

        [HttpPost("panel/update-reservation-user")]
        public IActionResult UpdateReservationLastStep(ReservationLastStepVM reservation)
        {
            try
            {
                var createReservation = HttpContext.Session.MyGet<ReservationDatasVM>("reservationData");

                if (createReservation == null)
                {
                    return NotFound();
                }

                var totalServiceFee = 0.0;
                if (reservation.ServiceList != null && reservation.ServiceList.Count > 0)
                {
                    foreach (var item1 in reservation.ServiceList)
                    {
                        var serviceFee = _serviceItems.SelectByID(item1.SelectedValue);
                        totalServiceFee += serviceFee.Price * item1.PeopleCountInput;
                    }
                }


                var prices = _tReservationHelpers.ReservationPrice(createReservation.LocationCar.Id, createReservation.KM, false, totalServiceFee, createReservation.ReservationValues.ReturnStatus, createReservation.IsOutZone, null, reservation.IsDiscount ? reservation.Discount : null);

                createReservation.LocationCar.Location = _location.SelectByID(createReservation.LocationCar.LocationId);

                var updatedService = createReservation.UpdateReservation;
                updatedService.DropLatLng = createReservation.DropLocationLatLng;
                updatedService.PickLatLng = createReservation.PickLocationLatLng;
                updatedService.Phone = reservation.PassengerPhone;
                updatedService.DropPlaceId = createReservation.DropLocationPlaceId;
                updatedService.PickPlaceId = createReservation.PickLocationPlaceId;
                updatedService.Email = reservation.PassengerEmail;

                updatedService.OfferPrice = prices.OfferPrice;
                updatedService.PartnerFee = prices.PartnerFee;
                updatedService.SalesFee = prices.SalesFee;
                updatedService.ServiceFee = prices.ServiceFee;
                updatedService.TotalPrice = prices.LastPrice;
                updatedService.GlobalPartnerFee = prices.GlobalPartnerFee;

                updatedService.Surname = reservation.PassengerSurname;
                updatedService.DropFullName = createReservation.DropLocationName;
                updatedService.PickFullName = createReservation.PickLocationName;
                updatedService.PeopleCount = createReservation.ReservationValues.PeopleCount;
                updatedService.ReservationDate = createReservation.ReservationValues.FlightTime;
                updatedService.ReturnDate = createReservation.ReservationValues.ReturnDate;
                updatedService.ReturnStatus = createReservation.ReservationValues.ReturnStatus;
                updatedService.DistanceText = createReservation.Distance;
                updatedService.DurationText = createReservation.Duration;
                updatedService.IsDiscount = reservation.IsDiscount;
                updatedService.Discount = reservation.Discount;
                updatedService.ExtraServiceFee = totalServiceFee;
                updatedService.Comment = reservation.PassengerComment;
                updatedService.HidePrice = reservation.HidePrice;
                updatedService.LocationCarId = createReservation.LocationCar.Id;
                updatedService.LastUpdate = DateTime.Now;

                var item = _reservations.Update(updatedService);

                if (createReservation.UpdateReservation.ReturnStatus)
                {
                    var returnReservation = _reservations.SelectByFunc(a => a.ReservationCode == updatedService.ReservationCode && a.IsThisReturn).FirstOrDefault();
                    if (returnReservation != null)
                    {
                        if (updatedService.ReturnStatus)
                        {
                            returnReservation.DropLatLng = updatedService.DropLatLng;
                            returnReservation.PickLatLng = updatedService.PickLatLng;
                            returnReservation.Phone = updatedService.Phone;
                            returnReservation.DropPlaceId = updatedService.DropPlaceId;
                            returnReservation.PickPlaceId = updatedService.PickPlaceId;
                            returnReservation.Email = updatedService.Email;
                            returnReservation.IsThisReturn = true;

                            returnReservation.OfferPrice = updatedService.OfferPrice;
                            returnReservation.PartnerFee = updatedService.PartnerFee;
                            returnReservation.SalesFee = updatedService.SalesFee;
                            returnReservation.ServiceFee = updatedService.ServiceFee;
                            returnReservation.TotalPrice = updatedService.TotalPrice;
                            returnReservation.GlobalPartnerFee = updatedService.GlobalPartnerFee;

                            returnReservation.Surname = updatedService.Surname;
                            returnReservation.DropFullName = updatedService.DropFullName;
                            returnReservation.PickFullName = updatedService.PickFullName;
                            returnReservation.PeopleCount = updatedService.PeopleCount;
                            returnReservation.ReservationDate = updatedService.ReturnDate;
                            returnReservation.ReturnDate = updatedService.ReturnDate;
                            returnReservation.ReturnStatus = false;
                            returnReservation.DistanceText = updatedService.DistanceText;
                            returnReservation.DurationText = updatedService.DurationText;
                            returnReservation.IsDiscount = updatedService.IsDiscount;
                            returnReservation.Discount = updatedService.Discount;
                            returnReservation.ExtraServiceFee = updatedService.ExtraServiceFee;
                            returnReservation.Comment = updatedService.Comment;
                            returnReservation.HidePrice = updatedService.HidePrice;
                            returnReservation.LocationCarId = updatedService.LocationCarId;
                            returnReservation.LastUpdate = DateTime.Now;

                            _reservations.Update(returnReservation);

                        }
                        else
                        {
                            _reservations.HardDelete(returnReservation);
                        }
                    }
                }
                else
                {
                    if (updatedService.ReturnStatus)
                    {
                        _reservations.Insert(new Reservations
                        {
                            DropLatLng = updatedService.DropLatLng,
                            PickLatLng = updatedService.PickLatLng,
                            Phone = updatedService.Phone,
                            DropPlaceId = updatedService.DropPlaceId,
                            PickPlaceId = updatedService.PickPlaceId,
                            Email = updatedService.Email,
                            LocationCarId = updatedService.LocationCarId,
                            Name = updatedService.Name,
                            ReservationCode = updatedService.ReservationCode,
                            Surname = updatedService.Surname,
                            DropFullName = updatedService.DropFullName,
                            PickFullName = updatedService.PickFullName,
                            PeopleCount = updatedService.PeopleCount,
                            ReservationDate = updatedService.ReturnDate,
                            ReturnDate = updatedService.ReturnDate,
                            ReturnStatus = false,
                            DistanceText = updatedService.DistanceText,
                            DurationText = updatedService.DurationText,
                            IsDiscount = updatedService.IsDiscount,
                            Discount = updatedService.Discount,
                            UserId = updatedService.UserId,
                            ExtraServiceFee = updatedService.ExtraServiceFee,
                            Comment = updatedService.Comment,
                            Status = updatedService.Status,
                            IsDelete = updatedService.IsDelete,
                            HidePrice = updatedService.HidePrice,
                            RealPhone = updatedService.RealPhone,
                            DiscountText = updatedService.DiscountText,
                            ReservationUserId = updatedService.ReservationUserId,
                            Rate = 0,
                            SalesAgencyId = updatedService.SalesAgencyId,
                            LastUpdate = DateTime.Now,
                            CreateDate = DateTime.Now,
                            IsThisReturn = true,

                            PartnerFee = updatedService.PartnerFee,
                            SalesFee = updatedService.SalesFee,
                            ServiceFee = updatedService.ServiceFee,
                            OfferPrice = updatedService.OfferPrice,
                            TotalPrice = updatedService.TotalPrice,
                            GlobalPartnerFee = updatedService.GlobalPartnerFee
                        });
                    }
                }

                var deleteServiceTable = _reservationServicesTable.SelectByFunc(a => a.ReservationId == updatedService.Id);
                deleteServiceTable.ForEach(a =>
                {
                    _reservationServicesTable.HardDelete(a);
                });

                var reservationItemsList = new List<ReservationServicesTable>();

                if (reservation.ServiceList != null && reservation.ServiceList.Count > 0)
                {
                    foreach (var item1 in reservation.ServiceList)
                    {
                        var serviceFee = _serviceItems.SelectByID(item1.SelectedValue);
                        reservationItemsList.Add(new ReservationServicesTable
                        {
                            ReservationId = item.Id,
                            PeopleCount = item1.PeopleCountInput,
                            Price = serviceFee.Price,
                            ServiceItemId = item1.SelectedValue,
                        });
                    }
                    _reservationServicesTable.InsertRage(reservationItemsList);
                }


                item.LocationCars = _locationCars.SelectByID(item.LocationCarId);
                item.LocationCars.Car = _carDetail.CarDetail(item.LocationCars.CarId);
                var loginAuth = _loginAuth.SelectByID(item.UserId);
                item.User = _userDatas.SelectByID(loginAuth.UserId);

                var deleteReservationPeople = _reservationPeople.SelectByFunc(a => a.ReservationId == updatedService.Id);
                deleteReservationPeople.ForEach(a =>
                {
                    _reservationPeople.HardDelete(a);
                });

                var reservationPeople = new List<ReservationPeople>();
                if (reservation.PassengerList != null && reservation.PassengerList.Count > 0)
                {
                    reservation.PassengerList.ForEach(a =>
                    {
                        reservationPeople.Add(new ReservationPeople
                        {
                            Name = a.PassengerName,
                            Surname = a.PassengerName,
                            ReservationId = item.Id
                        });
                    });

                    _reservationPeople.InsertRage(reservationPeople);
                }

                HttpContext.Session.Remove("reservationData");
                item.ReservationServicesTables = _reservationServicesTable.SelectByFunc(a => a.ReservationId == item.Id);
                item.ReservationServicesTables.ForEach(a =>
                {
                    a.ServiceItem = _serviceItems.SelectByID(a.ServiceItemId);
                    a.ServiceItem.ServiceProperty = _serviceProperties.SelectByID(a.ServiceItem.ServicePropertyId);
                });


                PdfCreator pdfCreator = new PdfCreator(_env);

                pdfCreator.CreateReservationPDF(createReservation.UpdateReservation.ReservationCode + "-" + createReservation.UpdateReservation.Id, item);


                item.ReservationServicesTables = _reservationServicesTable.SelectByFunc(a => a.ReservationId == item.Id);
                item.ReservationServicesTables.ForEach(a =>
                {
                    a.ServiceItem = _serviceItems.SelectByID(a.ServiceItemId);
                    a.ServiceItem.ServiceProperty = _serviceProperties.SelectByID(a.ServiceItem.ServicePropertyId);
                });

                return Json(new { result = 1, id = updatedService.Id });
            }
            catch (Exception)
            {
                ViewBag.Error = "Error";
                return RedirectToAction("Index", "Home");
            }

        }

    }
}
