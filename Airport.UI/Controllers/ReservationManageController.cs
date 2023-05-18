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

namespace Airport.UI.Controllers
{
    public class ReservationManageController : PanelAuthController
    {

        private readonly IWebHostEnvironment _env;

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
        public ReservationManageController(IReservationsDAL reservations, IDriversDAL drivers, IGetCarDetail carDetail, ILocationCarsDAL locationCars, IReservationPeopleDAL reservationPeople, ILocationsDAL locations, ILocationCarsFareDAL locationCarsFare, IUserDatasDAL userDatas, IServicesDAL services, IServiceItemsDAL serviceItems, IServicePropertiesDAL serviceProperties, IServiceCategoriesDAL serviceCategories, IReservationServicesTableDAL reservationServicesTable, IWebHostEnvironment env, IMail mail, ILoginAuthDAL loginAuth)
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
        }


        [HttpGet("panel/reservation-management")]
        public IActionResult Index()
        {
            try
            {
                var userId = Convert.ToInt32(Request.HttpContext.User.Claims.Where(a => a.Type == ClaimTypes.Sid).Select(a => a.Value).SingleOrDefault());
                var reservationVM = new ReservationsIndexVM()
                {
                    Reservations = _reservations.SelectByFunc(a => a.UserId == userId),
                    Drivers = _drivers.SelectByFunc(a => a.UserId == userId && !a.IsDelete)
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
                var reservation = _reservations.SelectByFunc(a => a.Id == id && a.UserId == userId).FirstOrDefault();
                if (reservation is not null)
                {
                    reservation.LocationCars = _locationCars.SelectByID(reservation.LocationCarId);
                    reservation.LocationCars.Car = _carDetail.CarDetail(reservation.LocationCars.CarId);
                    reservation.Driver = _drivers.SelectByID(reservation.DriverId);
                    reservation.ReservationPeoples = _reservationPeople.SelectByFunc(a => a.ReservationId == reservation.Id);
                    var reservationVM = new ReservationManagementVM()
                    {
                        Reservation = reservation,
                        Drivers = _drivers.SelectByFunc(a => a.UserId == userId && !a.IsDelete)
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

        public IActionResult CancelReservation(int id)
        {
            var userId = Convert.ToInt32(Request.HttpContext.User.Claims.Where(a => a.Type == ClaimTypes.Sid).Select(a => a.Value).SingleOrDefault());
            var getReservation = _reservations.SelectByFunc(a => a.Id == id && a.UserId == userId).FirstOrDefault();
            if (getReservation is not null)
            {
                getReservation.Status = 4;
                _reservations.Update(getReservation);

                return Json(new { result = 1 });
            }
            return BadRequest();
        }

        [HttpGet("panel/update-reservation/{id}")]
        public IActionResult UpdateReservationStepOne(int id)
        {
            try
            {
                var userId = Convert.ToInt32(Request.HttpContext.User.Claims.Where(a => a.Type == ClaimTypes.Sid).Select(a => a.Value).SingleOrDefault());
                var reservation = _reservations.SelectByFunc(a => a.Id == id && a.UserId == userId).FirstOrDefault();
                if (reservation != null)
                {
                    return View(reservation);
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

                var userId = Convert.ToInt32(Request.HttpContext.User.Claims.Where(a => a.Type == ClaimTypes.Sid).Select(a => a.Value).SingleOrDefault());

                var api_key = "AIzaSyAnqSEVlrvgHJymL-F8GmxIwNbe8fYUjdg";

                var httpClient = new HttpClient();

                var s = "https://maps.googleapis.com/maps/api/distancematrix/json?units=metric";
                var apiUrl = "https://maps.googleapis.com/maps/api/place/details/json?place_id=" + reservation.PickValue + "&key=" + api_key;
                var response = httpClient.GetAsync(apiUrl).Result;
                var content = response.Content.ReadAsStringAsync().Result;
                var contentJsonResult = JsonConvert.DeserializeObject<GetGoogleAPIVM>(content);


                var apiReturnUrl = "https://maps.googleapis.com/maps/api/place/details/json?place_id=" + reservation.DropValue + "&key=" + api_key;
                var returnResponse = httpClient.GetAsync(apiReturnUrl).Result;
                var content3 = returnResponse.Content.ReadAsStringAsync().Result;
                var contentJsonResult2 = JsonConvert.DeserializeObject<GetGoogleAPIVM>(content3);

                var fullUrl2 = s + $"&origins={contentJsonResult.Result.Geometry.Location.lat},{contentJsonResult.Result.Geometry.Location.lng}&destinations={contentJsonResult2.Result.Geometry.Location.lat},{contentJsonResult2.Result.Geometry.Location.lng}&key=" + api_key;
                HttpResponseMessage response4 = httpClient.GetAsync(fullUrl2).Result;
                var content4 = response4.Content.ReadAsStringAsync().Result;
                var betweenLocation = JsonConvert.DeserializeObject<DistanceMatrixApiResponse>(content4);

                if (betweenLocation.status == "OK")
                {
                    var locations = _location.SelectByFunc(a => !a.IsDelete && a.UserId == userId);
                    var listlocation = new List<ReservationLocationCarsVM>();
                    var locationCars = new List<List<ReservationLocationCarsVM>>();

                    locations.ForEach(a =>
                    {
                        a.LocationCars = _locationCars.SelectByFunc(b => b.LocationId == a.Id);

                        listlocation.Add(new ReservationLocationCarsVM
                        {
                            LocationCar = a.LocationCars,
                            PlaceId = a.LocationMapId,
                            ZoneValue = a.LocationRadius,
                            Lat = a.Lat,
                            Lng = a.Lng
                        });
                    });

                    if (listlocation.Count != 0)
                    {
                        locationCars.Add(listlocation);
                    }

                    var allDatas = new List<AllDatas>();
                    locationCars.ForEach(a =>
                    {
                        string carLatLngString = "";
                        a.ForEach(b =>
                        {
                            carLatLngString += b.Lat + "," + b.Lng + "|";
                        });

                        var apiArrayUrl = $"https://maps.googleapis.com/maps/api/distancematrix/json?units=metric&origins={contentJsonResult.Result.Geometry.Location.lat + "," + contentJsonResult.Result.Geometry.Location.lng}&destinations={carLatLngString}&key=" + api_key;
                        var arrayResponse = httpClient.GetAsync(apiArrayUrl).Result;
                        var arrayContent = arrayResponse.Content.ReadAsStringAsync().Result;
                        var data = JsonConvert.DeserializeObject<DistanceMatrixApiResponse>(arrayContent);
                        if (data.status == "OK")
                        {
                            int i2 = 0;
                            var locationdatas = carLatLngString.Split("|");
                            data.destination_addresses.ForEach(a =>
                            {
                                if (data.rows[0].elements[i2].status == "OK")
                                {
                                    allDatas.Add(new AllDatas
                                    {
                                        DisanceValue = data.rows[0].elements[i2].distance.value.ToString(),
                                        DurationeValue = data.rows[0].elements[i2].distance.value.ToString(),
                                        Lat = locationdatas[i2].Split(",")[0],
                                        Lng = locationdatas[i2].Split(",")[1],
                                        Destinationaddresses = data.destination_addresses[i2]
                                    });
                                }
                                i2++;
                            });
                        }
                    });

                    var selectedLocations = new List<LocationIsOutVM>();
                    allDatas = allDatas.Distinct().ToList();
                    allDatas.ForEach(a =>
                    {
                        var convertLocation = _location.SelectByFunc(b => b.Lat == a.Lat && b.Lng == a.Lng && !b.IsDelete && b.UserId == userId);
                        convertLocation.ForEach(b =>
                        {
                            var realRadiusValue = Convert.ToInt32(b.LocationRadius) * 1000;
                            if (realRadiusValue > Convert.ToInt32(a.DisanceValue))
                            {
                                selectedLocations.Add(new LocationIsOutVM
                                {
                                    IsOutZone = false,
                                    Location = b
                                });

                            }
                            else if (b.OutZonePricePerKM > 0 && realRadiusValue < Convert.ToInt32(a.DisanceValue))
                            {
                                selectedLocations.Add(new LocationIsOutVM
                                {
                                    IsOutZone = true,
                                    Location = b
                                });
                            }
                        });
                    });
                    var getreservation = new List<GetReservationValues>();
                    selectedLocations = selectedLocations.Distinct().ToList();
                    double minKm = 0;

                    var selectedLocationsMini = new List<LocationIsOutMiniVM>();

                    if (betweenLocation.rows[0].elements[0].status == "OK")
                    {

                        var lastKm = Math.Round(Convert.ToDouble(betweenLocation.rows[0].elements[0].distance.value) / 100) * 100;
                        minKm = lastKm / 1000;
                        selectedLocations.ForEach(a =>
                        {

                            a.Location.LocationCars = _locationCars.SelectByFunc(b => b.LocationId == a.Location.Id);

                            a.Location.LocationCars.ForEach(b =>
                            {
                                selectedLocationsMini.Add(new LocationIsOutMiniVM
                                {
                                    LocationCarId = b.Id,
                                    IsOutZone = a.IsOutZone
                                });

                                b.Car = _carDetail.CarDetail(b.CarId);
                                if (b.Car.MaxPassenger >= reservation.PeopleCount)
                                {
                                    double price = 0;
                                    if (a.IsOutZone)
                                    {
                                        price = a.Location.DropCharge + (minKm * a.Location.OutZonePricePerKM);
                                    }
                                    else
                                    {
                                        b.LocationCarsFares = _locationCarsFare.SelectByFunc(c => c.LocationCarId == b.Id);
                                        var lastUp = 0;
                                        double lastPrice = 0;

                                        b.LocationCarsFares.ForEach(c =>
                                        {
                                            var fare = Convert.ToDouble(c.Fare);
                                            if (c.StartFrom < minKm)
                                            {
                                                if (c.PriceType == 2)
                                                {
                                                    price += fare * (c.UpTo - c.StartFrom);
                                                }
                                                else
                                                {

                                                    price += fare;
                                                }
                                            }

                                            lastUp = c.UpTo;
                                            lastPrice = fare;
                                        });


                                        if (lastUp < minKm)
                                        {
                                            var plusPrice = minKm - lastUp;
                                            price += lastPrice * plusPrice;
                                        }                                    
                                    }

                                    if (reservation.ReturnStatus)
                                    {
                                        price *= 2;
                                    }

                                    price = Math.Round(price,2);
                                    getreservation.Add(new GetReservationValues
                                    {
                                        LocationCars = b,
                                        LastPrice = price,
                                        ReservationDate = reservation.FlightTime,
                                        PickLocationName = contentJsonResult.Result.formatted_address,
                                        DropLocationName = contentJsonResult2.Result.formatted_address,
                                        PassangerCount = reservation.PeopleCount,
                                        DropLocationLatLng = $"{contentJsonResult.Result.Geometry.Location.lat},{contentJsonResult.Result.Geometry.Location.lng}",
                                        PickLocationLatLng = $"{contentJsonResult2.Result.Geometry.Location.lat},{contentJsonResult2.Result.Geometry.Location.lng}",
                                        DropLocationPlaceId = reservation.DropValue,
                                        PickLocationPlaceId = reservation.PickValue,
                                    });
                                }
                            });
                        });
                    }
                      
                    var updateReservation = _reservations.SelectByID(id);

                    getreservation = getreservation.Distinct().ToList();

                    var lastVM = new ReservationStepTwoVM()
                    {
                        ReservationValues = getreservation,
                        DropLocationLatLng = $"lat:{contentJsonResult.Result.Geometry.Location.lat},lng:{contentJsonResult.Result.Geometry.Location.lng}",
                        PickLocationLatLng = $"lat:{contentJsonResult2.Result.Geometry.Location.lat},lng:{contentJsonResult2.Result.Geometry.Location.lng}",
                        DropLocationPlaceId = reservation.DropValue,
                        PickLocationPlaceId = reservation.PickValue,
                        Distance = betweenLocation.rows[0].elements[0].distance.text,
                        Duration = betweenLocation.rows[0].elements[0].duration.text,
                        SelectedReservationValues = reservation,
                        UpdateReservationValues = updateReservation
                    };

                    var reservationDatas = new ReservationDatasVM()
                    {
                        DropLocationLatLng = $"lat:{contentJsonResult.Result.Geometry.Location.lat},lng:{contentJsonResult.Result.Geometry.Location.lng}",
                        PickLocationLatLng = $"lat:{contentJsonResult2.Result.Geometry.Location.lat},lng:{contentJsonResult2.Result.Geometry.Location.lng}",
                        DropLocationPlaceId = reservation.DropValue,
                        PickLocationPlaceId = reservation.PickValue,
                        PickLocationName = contentJsonResult.Result.formatted_address,
                        DropLocationName = contentJsonResult2.Result.formatted_address,
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


                return RedirectToAction("Index", "Home");
                //return View(lastVM);
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
                var userId = Convert.ToInt32(Request.HttpContext.User.Claims.Where(a => a.Type == ClaimTypes.Sid).Select(a => a.Value).SingleOrDefault());
                var user = _userDatas.SelectByID(userId);

                var datas = HttpContext.Session.MyGet<ReservationDatasVM>("reservationData");

                if (datas == null)
                {
                    return NotFound();
                }

                var selectedDatasMini = HttpContext.Session.MyGet<List<LocationIsOutMiniVM>>("selectedLocationMini").Where(a => a.LocationCarId == id).FirstOrDefault();

                if (selectedDatasMini != null)
                {
                    var locationCar = _locationCars.SelectByID(selectedDatasMini.LocationCarId);

                    locationCar.LocationCarsFares = _locationCarsFare.SelectByFunc(a => a.LocationCarId == id);
                    locationCar.Location = _location.SelectByID(locationCar.LocationId);

                    double price = 0;
                    var lastUp = 0;
                    double lastPrice = 0;
                    if (selectedDatasMini.IsOutZone)
                    {
                        price = locationCar.Location.DropCharge + (datas.KM * locationCar.Location.OutZonePricePerKM);
                    }
                    else
                    {
                        locationCar.LocationCarsFares = _locationCarsFare.SelectByFunc(c => c.LocationCarId == locationCar.Id);
                        locationCar.LocationCarsFares.ForEach(c =>
                        {
                            var fare = Convert.ToDouble(c.Fare);
                            if (c.StartFrom < datas.KM)
                            {
                                if (c.PriceType == 2)
                                {
                                    price += fare * (c.UpTo - c.StartFrom);
                                }
                                else
                                {
                                    price += fare;
                                }
                            }

                            lastUp = c.UpTo;
                            lastPrice = fare;
                        });

                        if (lastUp < datas.KM)
                        {
                            var plusPrice = datas.KM - lastUp;
                            price += lastPrice * plusPrice;
                        }
                    }

                    if (datas.ReservationValues.ReturnStatus)
                    {
                        price *= 2;
                    }

                    price = Math.Round(price, 2);
                    datas.LastPrice = price;
                    datas.LocationCar = locationCar;
                    datas.LocationCar.Car = _carDetail.CarDetail(locationCar.CarId);
                    datas.LocationCar.Car.Service = _services.SelectByID(datas.LocationCar.Car.ServiceId);
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
                    datas.UpdateReservation.ReservationServicesTables = _reservationServicesTable.SelectByFunc(a=>a.ReservationId == datas.UpdateReservation.Id);
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

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error";
                return BadRequest(ex.ToString());
            }

        }

        [HttpPost("panel/update-reservation-user/{id}", Name = "UpdateReservationForm")]
        public IActionResult UpdateReservationLastStep(Reservations reservation, List<string> OthersName, List<string> OthersSurname, List<int> serviceItems, string selectedServiceItems)
        {
            try
            {
                var createReservation = HttpContext.Session.MyGet<ReservationDatasVM>("reservationData");

                if (createReservation == null)
                {
                    return NotFound();
                }

                var peopleList = new List<GetOtherPeople>();
                for (int i = 0; i < OthersName.Count; i++)
                {
                    if (OthersName[i].Trim() != "" && OthersName[i] != null)
                    {
                        peopleList.Add(new GetOtherPeople
                        {
                            OthersName = OthersName[i],
                            OthersSurname = OthersSurname[i]
                        });
                    }
                }

                var totalServiceFee = 0.0;
                var services = new List<SelectServiceVM>();
                if (selectedServiceItems != null)
                {
                    services = JsonConvert.DeserializeObject<List<SelectServiceVM>>(selectedServiceItems);
                    foreach (var item1 in services)
                    {
                        var serviceFee = _serviceItems.SelectByID(item1.SelectedValue);
                        totalServiceFee += serviceFee.Price * item1.PeopleCountInput;
                    }
                }

                createReservation.LocationCar.Location = _location.SelectByID(createReservation.LocationCar.LocationId);

                var updatedService = createReservation.UpdateReservation;
                updatedService.DropLatLng = createReservation.DropLocationLatLng;
                updatedService.PickLatLng = createReservation.PickLocationLatLng;
                updatedService.Phone = reservation.Phone;
                updatedService.DropPlaceId = createReservation.DropLocationPlaceId;
                updatedService.PickPlaceId = createReservation.PickLocationPlaceId;
                updatedService.Email = reservation.Email;
                updatedService.OfferPrice = createReservation.LastPrice;
                updatedService.Surname = reservation.Surname;
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
                updatedService.ServiceFee = totalServiceFee;
                updatedService.Comment = reservation.Comment;
                updatedService.Discount = reservation.Discount;
                updatedService.HidePrice = reservation.HidePrice;
                updatedService.LocationCarId = createReservation.LocationCar.Id;

                var item = _reservations.Update(updatedService);

                var reservationItemsList = new List<ReservationServicesTable>();

                foreach (var item1 in services)
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

                var deleteServiceTable = _reservationServicesTable.SelectByFunc(a=>a.ReservationId == updatedService.Id);
                deleteServiceTable.ForEach(a =>
                {
                    _reservationServicesTable.HardDelete(a);
                });

                _reservationServicesTable.InsertRage(reservationItemsList);

                item.LocationCars = _locationCars.SelectByID(item.LocationCarId);
                item.LocationCars.Car = _carDetail.CarDetail(item.LocationCars.CarId);
                var loginAuth = _loginAuth.SelectByID(item.UserId);
                item.User = _userDatas.SelectByID(loginAuth.UserId);

                var reservationPeople = new List<ReservationPeople>();

                peopleList.ForEach(a =>
                {
                    reservationPeople.Add(new ReservationPeople
                    {
                        Name = a.OthersName,
                        Surname = a.OthersSurname,
                        ReservationId = item.Id
                    });
                });

                var deleteReservationPeople = _reservationPeople.SelectByFunc(a => a.ReservationId == updatedService.Id);
                deleteReservationPeople.ForEach(a =>
                {
                    _reservationPeople.HardDelete(a);
                });

                _reservationPeople.InsertRage(reservationPeople);



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

                return View(item);
            }
            catch (Exception)
            {
                ViewBag.Error = "Error";
                return RedirectToAction("Index", "Home");
            }

        }

    }
}
