using Airport.DBEntities.Entities;
using Airport.DBEntitiesDAL.Interfaces;
using Airport.MessageExtensions.Interfaces;
using Airport.UI.Models.Extendions;
using Airport.UI.Models.IM;
using Airport.UI.Models.Interface;
using Airport.UI.Models.VM;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Airport.MessageExtension.VM;
using iText.Layout.Element;
using Airport.MessageExtension.Interfaces;
using System.Globalization;
using System.IO;

namespace Airport.UI.Controllers
{
    public class ReservationController : Controller
    {
        private readonly IWebHostEnvironment _env;

        ILocationsDAL _location;
        ILocationCarsDAL _locationCar;
        ILocationCarsFareDAL _locationCarsFare;
        IGetCarDetail _carDetail;
        IUserDatasDAL _userDatas;
        IReservationsDAL _reservations;
        IGetCarDetail _getCar;
        IReservationPeopleDAL _reservationsPeople;
        IMail _mail;
        IServicesDAL _services;
        IServiceItemsDAL _serviceItems;
        IServicePropertiesDAL _serviceProperties;
        IServiceCategoriesDAL _serviceCategories;
        IReservationServicesTableDAL _reservationServicesTable;
        ICouponsDAL _coupons;
        ISMS _sms;
        ILoginAuthDAL _loginAuth;

        public ReservationController(ILocationsDAL location, ILocationCarsDAL locationCar, ILocationCarsFareDAL locationCarsFare, IGetCarDetail carDetail, IUserDatasDAL userDatas, IReservationsDAL reservations, IGetCarDetail getCar, IReservationPeopleDAL reservationsPeople, IMail mail, IWebHostEnvironment env, IServicesDAL services, IServiceItemsDAL serviceItems, IServicePropertiesDAL serviceProperties, IServiceCategoriesDAL serviceCategories, IReservationServicesTableDAL reservationServicesTable, ICouponsDAL coupons,ISMS sms, ILoginAuthDAL loginAuth)
        {
            _location = location;
            _locationCar = locationCar;
            _locationCarsFare = locationCarsFare;
            _carDetail = carDetail;
            _userDatas = userDatas;
            _reservations = reservations;
            _getCar = getCar;
            _reservationsPeople = reservationsPeople;
            _mail = mail;
            _env = env;
            _services = services;
            _serviceItems = serviceItems;
            _serviceProperties = serviceProperties;
            _serviceCategories = serviceCategories;
            _reservationServicesTable = reservationServicesTable;
            _coupons = coupons;
            _sms = sms;
            _loginAuth = loginAuth;
        }

        [HttpPost("reservation", Name = "getLocationValue")]
        public async Task<IActionResult> ReservationStepTwo(GetResevationIM reservation)
        {
            try
            {
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
                    var locations = _location.SelectByFunc(a => !a.IsDelete);
                    var listlocation = new List<ReservationLocationCarsVM>();
                    var locationCars = new List<List<ReservationLocationCarsVM>>();
                    int i = 0;

                    locations.ForEach(a =>
                    {
                        a.LocationCars = _locationCar.SelectByFunc(b => b.LocationId == a.Id);

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
                        var convertLocation = _location.SelectByFunc(b => b.Lat == a.Lat && b.Lng == a.Lng && !b.IsDelete);
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
                            a.Location.LocationCars = _locationCar.SelectByFunc(b => b.LocationId == a.Location.Id);

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
                                                    if (c.UpTo < minKm)
                                                    {
                                                        price += fare * (c.UpTo - c.StartFrom);
                                                    }
                                                    else
                                                    {
                                                        price += fare * minKm;
                                                    }
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
                                    price = Math.Round(price, 2);
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
                    

                    getreservation = getreservation.Distinct().ToList();

                    var lastVM = new ReservationStepTwoVM()
                    {
                        ReservationValues = getreservation,
                        DropLocationLatLng = $"lat:{contentJsonResult.Result.Geometry.Location.lat},lng:{contentJsonResult.Result.Geometry.Location.lng}",
                        PickLocationLatLng = $"lat:{contentJsonResult2.Result.Geometry.Location.lat},lng:{contentJsonResult2.Result.Geometry.Location.lng}",
                        DropLocationPlaceId = reservation.DropValue,
                        PickLocationPlaceId = reservation.PickValue,
                        Distance = betweenLocation.rows[0].elements[0].distance?.text,
                        Duration = betweenLocation.rows[0].elements[0].duration?.text,
                        SelectedReservationValues = reservation
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
                        Distance = betweenLocation.rows[0].elements[0].distance?.text,
                        Duration = betweenLocation.rows[0].elements[0].duration?.text
                    };



                    HttpContext.Session.Remove("reservationData");
                    HttpContext.Session.MySet("reservationData", reservationDatas);
                    HttpContext.Session.MySet("selectedLocationMini", selectedLocationsMini);


                    lastVM.ReservationValues = lastVM.ReservationValues.OrderBy(a => a.LastPrice).ToList();



                    ViewBag.test = locationCars;
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

        [HttpGet("reservation-step-three/{id}")]
        public async Task<IActionResult> ReservationStepThree(int id)
        {
            try
            {
                var userId = Convert.ToInt32(Request.HttpContext.User.Claims.Where(a => a.Type == ClaimTypes.Sid).Select(a => a.Value).SingleOrDefault());
                var loginAuth = _loginAuth.SelectByID(userId);
                var user = _userDatas.SelectByID(loginAuth.UserId);
                user.LoginAuth = loginAuth;

                var datas = HttpContext.Session.MyGet<ReservationDatasVM>("reservationData");

                if (datas == null)
                {
                    return NotFound();
                }

                var selectedDatasMini = HttpContext.Session.MyGet<List<LocationIsOutMiniVM>>("selectedLocationMini").Where(a => a.LocationCarId == id).FirstOrDefault();

                if (selectedDatasMini != null)
                {
                    var locationCar = _locationCar.SelectByID(selectedDatasMini.LocationCarId);


                    locationCar.LocationCarsFares = _locationCarsFare.SelectByFunc(a => a.LocationCarId == id);
                    locationCar.Location = _location.SelectByID(locationCar.LocationId);

                    //fixed= 1
                    //per = 2 

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
                                    if (c.UpTo < datas.KM)
                                    {
                                        price += fare * (c.UpTo - c.StartFrom);
                                    }
                                    else
                                    {
                                        price += fare * datas.KM;
                                    }
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

                        datas.LocationCar.Car.Service.ServiceItems.ForEach(a =>
                        {
                            a.ServiceProperty = _serviceProperties.SelectByID(a.ServicePropertyId);
                        });

                    }


                    HttpContext.Session.Remove("reservationData");

                    HttpContext.Session.MySet("reservationData", datas);

                    var reservation = new ReservationStepThreeVM()
                    {
                        SelectedData = datas,
                        User = user,
                        ServiceItems = selectedCarItems
                    };

                    return View(reservation);
                }

                return RedirectToAction("Index", "Home");
            }
            catch (Exception)
            {
                ViewBag.Error = "Error";
                return RedirectToAction("Index", "Home");
            }

        }

        [HttpPost("reservation-get-code", Name = "getBookValues")]
        public async Task<IActionResult> ReservationLastStep(Reservations reservation, List<string> OthersName, List<string> OthersSurname, List<int> serviceItems, string selectedServiceItems,string coupon)
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

                Random random = new Random();
                int kodUzunlugu = 6;
                string karakterler = "0123456789";
                string kod = "";
                for (int i = 0; i < kodUzunlugu; i++)
                {
                    int index = random.Next(karakterler.Length);
                    kod += karakterler[index];
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

                var getCoupon = _coupons.SelectByFunc(a => a.Active && a.CouponCode == coupon && a.CouponStartDate <= DateTime.Now
                                                                                        && a.CouponFinishDate >= DateTime.Now).FirstOrDefault();
                var total = createReservation.LastPrice + totalServiceFee;

                if (getCoupon is not null)
                {
                    total = total - ((total * getCoupon.Discount) / 100);
                }

                total = Math.Round(total, 2);

                var item = _reservations.Insert(new Reservations
                {
                    DropLatLng = createReservation.DropLocationLatLng,
                    PickLatLng = createReservation.PickLocationLatLng,
                    Phone = reservation.Phone,
                    DropPlaceId = createReservation.DropLocationPlaceId,
                    PickPlaceId = createReservation.PickLocationPlaceId,
                    Email = reservation.Email,
                    LocationCarId = createReservation.LocationCar.Id,
                    Name = reservation.Name,
                    ReservationCode = kod,
                    OfferPrice = createReservation.LastPrice,
                    Surname = reservation.Surname,
                    DropFullName = createReservation.DropLocationName,
                    PickFullName = createReservation.PickLocationName,
                    PeopleCount = createReservation.ReservationValues.PeopleCount,
                    ReservationDate = createReservation.ReservationValues.FlightTime,
                    ReturnDate = createReservation.ReservationValues.ReturnDate,
                    ReturnStatus = createReservation.ReservationValues.ReturnStatus,
                    DistanceText = createReservation.Distance,
                    DurationText = createReservation.Duration,
                    Discount = 0,
                    IsDiscount = false,
                    UserId = _location.SelectByID(createReservation.LocationCar.LocationId).UserId,
                    ServiceFee = totalServiceFee,
                    Comment = reservation.Comment,
                    Status = 1,
                    IsDelete = false,
                    HidePrice = reservation.HidePrice,
                    Coupon = getCoupon?.Id,
                    TotalPrice = total,
                    RealPhone = reservation.RealPhone,
                });

                item.Coupons = getCoupon;

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

                _reservationServicesTable.InsertRage(reservationItemsList);

                item.LocationCars = _locationCar.SelectByID(item.LocationCarId);
                item.LocationCars.Car = _getCar.CarDetail(item.LocationCars.CarId);

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

                _reservationsPeople.InsertRage(reservationPeople);


                HttpContext.Session.Remove("reservationData");
                item.ReservationServicesTables = _reservationServicesTable.SelectByFunc(a => a.ReservationId == item.Id);
                item.ReservationServicesTables.ForEach(a =>
                {
                    a.ServiceItem = _serviceItems.SelectByID(a.ServiceItemId);
                    a.ServiceItem.ServiceProperty = _serviceProperties.SelectByID(a.ServiceItem.ServicePropertyId);
                });

                PdfCreator pdfCreator = new PdfCreator(_env);
                pdfCreator.CreateReservationPDF(kod + "-" + item.Id, item);


                _mail.SendReservationMail(item);

                var allMessage = new List<Mesaj>();
                allMessage.Add(new Mesaj
                {
                    dest = reservation.RealPhone,
                    msg = @$"Your reservation code {item.ReservationCode} has been created. Voucher Link http://test.airportglobaltransfer.com/pdf/{item.ReservationCode}-{item.Id}.pdf"
                });

                var mesaj = allMessage.ToArray();

                _sms.SmsForReservation(mesaj);

                item.ReservationServicesTables = _reservationServicesTable.SelectByFunc(a => a.ReservationId == item.Id);
                item.ReservationServicesTables.ForEach(a =>
                {
                    a.ServiceItem = _serviceItems.SelectByID(a.ServiceItemId);
                    a.ServiceItem.ServiceProperty = _serviceProperties.SelectByID(a.ServiceItem.ServicePropertyId);
                });
                return View(item);
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

        [HttpPost("manage-reservation", Name = "checkReservation")]
        public async Task<IActionResult> GetReservation(string reservationCode, string email)
        {
            try
            {
                var reservation = _reservations.SelectByFunc(a => a.Email == email && a.ReservationCode == reservationCode).FirstOrDefault();
                if (reservation == null)
                {
                    ViewBag.Warning = "Warning";
                    return RedirectToAction("ManageReservation", "Home");
                }

                reservation.LocationCars = _locationCar.SelectByID(reservation.LocationCarId);
                reservation.LocationCars.Car = _getCar.CarDetail(reservation.LocationCars.CarId);

                return View(reservation);
            }
            catch (Exception)
            {
                ViewBag.Error = "Error";
                return RedirectToAction("Index", "Home");
            }

        }

        [HttpGet("panel/manual-reservation-one")]
        public async Task<IActionResult> ManualReservationStepOne()
        {
            return View();
        }

        [HttpPost("panel/manual-reservation-two", Name = "getManualLocationValue")]
        public async Task<IActionResult> ManualReservationStepTwo(GetResevationIM reservation)
        {
            try
            {

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
                    var locations = _location.SelectByFunc(a => !a.IsDelete);
                    var listlocation = new List<ReservationLocationCarsVM>();
                    var locationCars = new List<List<ReservationLocationCarsVM>>();
                    int i = 0;

                    locations.ForEach(a =>
                    {
                        a.LocationCars = _locationCar.SelectByFunc(b => b.LocationId == a.Id);

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

                    allDatas.ForEach(a =>
                    {
                        var convertLocation = _location.SelectByFunc(b => b.Lat == a.Lat && b.Lng == a.Lng && !b.IsDelete);
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
                    var selectedLocationsMini = new List<LocationIsOutMiniVM>();
                    double minKm = 0;

                    if (betweenLocation.rows[0].elements[0].status == "OK")
                    {
                        var lastKm = Math.Round(Convert.ToDouble(betweenLocation.rows[0].elements[0].distance.value) / 100) * 100;
                        minKm = lastKm / 1000;
                        selectedLocations.ForEach(a =>
                        {
                            a.Location.LocationCars = _locationCar.SelectByFunc(b => b.LocationId == a.Location.Id);

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
                                                    if (c.UpTo < minKm)
                                                    {
                                                        price += fare * (c.UpTo - c.StartFrom);
                                                    }
                                                    else
                                                    {
                                                        price += fare * minKm;
                                                    }
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

                                    price = Math.Round(price, 2);
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
                   


                    getreservation = getreservation.Distinct().ToList();

                    var lastVM = new ReservationStepTwoVM()
                    {
                        ReservationValues = getreservation,
                        DropLocationLatLng = $"lat:{contentJsonResult.Result.Geometry.Location.lat},lng:{contentJsonResult.Result.Geometry.Location.lng}",
                        PickLocationLatLng = $"lat:{contentJsonResult2.Result.Geometry.Location.lat},lng:{contentJsonResult2.Result.Geometry.Location.lng}",
                        DropLocationPlaceId = reservation.DropValue,
                        PickLocationPlaceId = reservation.PickValue,
                        Distance = betweenLocation.rows[0].elements[0].distance?.text,
                        Duration = betweenLocation.rows[0].elements[0].duration?.text,
                        SelectedReservationValues = reservation
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
                        Distance = betweenLocation.rows[0].elements[0].distance?.text,
                        Duration = betweenLocation.rows[0].elements[0].duration?.text
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

        [HttpGet("panel/manual-reservation-three/{id}")]
        public IActionResult ManualReservationStepThree(int id)
        {
            try
            {
                var userId = Convert.ToInt32(Request.HttpContext.User.Claims.Where(a => a.Type == ClaimTypes.Sid).Select(a => a.Value).SingleOrDefault());
                var loginAuth = _loginAuth.SelectByID(userId);
                var user = _userDatas.SelectByID(loginAuth.UserId);
                user.LoginAuth = loginAuth;

                var datas = HttpContext.Session.MyGet<ReservationDatasVM>("reservationData");

                if (datas == null)
                {
                    return NotFound();
                }


                var selectedDatasMini = HttpContext.Session.MyGet<List<LocationIsOutMiniVM>>("selectedLocationMini").Where(a => a.LocationCarId == id).FirstOrDefault();

                if (selectedDatasMini != null)
                {
                    var locationCar = _locationCar.SelectByID(selectedDatasMini.LocationCarId);


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
                                    if (c.UpTo < datas.KM)
                                    {
                                        price += fare * (c.UpTo - c.StartFrom);
                                    }
                                    else
                                    {
                                        price += fare * datas.KM;
                                    }
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

                    var reservation = new ReservationStepThreeVM()
                    {
                        SelectedData = datas,
                        User = user,
                        ServiceItems = selectedCarItems
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

        [HttpPost("panel/manual-reservation-three/{id}", Name = "getManualBookValues")]
        public IActionResult ManualReservationLastStep(Reservations reservation, List<string> OthersName, List<string> OthersSurname, List<int> serviceItems, string selectedServiceItems)
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

                Random random = new Random();
                int kodUzunlugu = 6;
                string karakterler = "0123456789";
                string kod = "";
                for (int i = 0; i < kodUzunlugu; i++)
                {
                    int index = random.Next(karakterler.Length);
                    kod += karakterler[index];
                }
                createReservation.LocationCar.Location = _location.SelectByID(createReservation.LocationCar.LocationId);


                var totalprice = reservation.IsDiscount ? Convert.ToDouble(reservation.Discount) : createReservation.LastPrice + totalServiceFee;

                var item = _reservations.Insert(new Reservations
                {
                    DropLatLng = createReservation.DropLocationLatLng,
                    PickLatLng = createReservation.PickLocationLatLng,
                    Phone = reservation.Phone,
                    DropPlaceId = createReservation.DropLocationPlaceId,
                    PickPlaceId = createReservation.PickLocationPlaceId,
                    Email = reservation.Email,
                    LocationCarId = createReservation.LocationCar.Id,
                    Name = reservation.Name,
                    ReservationCode = kod,
                    OfferPrice = createReservation.LastPrice,
                    Surname = reservation.Surname,
                    DropFullName = createReservation.DropLocationName,
                    PickFullName = createReservation.PickLocationName,
                    PeopleCount = createReservation.ReservationValues.PeopleCount,
                    ReservationDate = createReservation.ReservationValues.FlightTime,
                    ReturnDate = createReservation.ReservationValues.ReturnDate,
                    ReturnStatus = createReservation.ReservationValues.ReturnStatus,
                    DistanceText = createReservation.Distance,
                    DurationText = createReservation.Duration,
                    IsDiscount = reservation.IsDiscount,
                    Discount = reservation.Discount,
                    UserId = _location.SelectByID(createReservation.LocationCar.LocationId).UserId,
                    ServiceFee = totalServiceFee,
                    Comment = reservation.Comment,
                    Status = 1,
                    IsDelete = false,
                    HidePrice = reservation.HidePrice,
                    TotalPrice = totalprice,
                    RealPhone = reservation.RealPhone,
                    
                });

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

                _reservationServicesTable.InsertRage(reservationItemsList);


                item.LocationCars = _locationCar.SelectByID(item.LocationCarId);
                item.LocationCars.Car = _getCar.CarDetail(item.LocationCars.CarId);

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

                _reservationsPeople.InsertRage(reservationPeople);



                HttpContext.Session.Remove("reservationData");
                item.ReservationServicesTables = _reservationServicesTable.SelectByFunc(a => a.ReservationId == item.Id);
                item.ReservationServicesTables.ForEach(a =>
                {
                    a.ServiceItem = _serviceItems.SelectByID(a.ServiceItemId);
                    a.ServiceItem.ServiceProperty = _serviceProperties.SelectByID(a.ServiceItem.ServicePropertyId);
                });



                PdfCreator pdfCreator = new PdfCreator(_env);
                pdfCreator.CreateReservationPDF(kod + "-" + item.Id, item);

                _mail.SendReservationMail(item);

                var allMessage = new List<Mesaj>();
                allMessage.Add(new Mesaj
                {
                    dest = reservation.RealPhone,
                    msg = @$"Your reservation code {item.ReservationCode} has been created. Voucher Link http://test.airportglobaltransfer.com/pdf/{item.ReservationCode}-{item.Id}.pdf"
                });

                var mesaj = allMessage.ToArray();

                _sms.SmsForReservation(mesaj);

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

        public JsonResult GetServiceItem(int[] serviceProId, int serviceId)
        {
            try
            {
                var list = new List<int>();
                var serviceVM = new List<GetServiceItemDetailVM>();
                var serviceVM2 = new List<GetServiceCategoryItemVM>();
                foreach (var item in serviceProId)
                {
                    var serviceProp = _serviceProperties.SelectByID(item);

                    if (serviceProp != null)
                    {
                        list.Add(serviceProp.ServiceCategoryId);
                        serviceProp.ServiceCategory = _serviceCategories.SelectByID(serviceProp?.ServiceCategoryId);
                        serviceVM2.Add(new GetServiceCategoryItemVM()
                        {
                            Id = serviceProp.Id,
                            ServiceDescripton = serviceProp.ServicePropertyDescription,
                            ServiceCategoryName = serviceProp.ServiceCategory.ServiceCategoryName,
                            ServiceName = serviceProp.ServicePropertyName,
                            ServiceCategoryId = serviceProp.ServiceCategoryId,
                            Price = _serviceItems.SelectByFunc(a => a.ServiceId == serviceId && a.ServicePropertyId == serviceProp.Id).FirstOrDefault()?.Price
                        });
                    }
                }

                list = list.Distinct().ToList();

                list.ForEach(a =>
                {
                    serviceVM.Add(new GetServiceItemDetailVM
                    {
                        ServiceCategoryId = a,
                        ServiceCategoryName = serviceVM2.Where(b => b.ServiceCategoryId == a).FirstOrDefault().ServiceCategoryName,
                        CategoryItems = serviceVM2.Where(b => b.ServiceCategoryId == a).ToList()
                    });
                });



                return new JsonResult(new { result = 1, data = serviceVM });
            }
            catch (System.Exception)
            {
                return new JsonResult(new { });
            }
        }


        public JsonResult CheckCoupon(string coupon)
        {
            try
            {
                var coupons = _coupons.SelectByFunc(a => a.Active && a.CouponCode == coupon && a.CouponStartDate <= DateTime.Now 
                                                                                        && a.CouponFinishDate >= DateTime.Now).FirstOrDefault();
                if (coupons != null)
                {
                    return new JsonResult(new { result = 1, data = coupons });
                }

                return new JsonResult(new { result = 2 });
            }
            catch (System.Exception)
            {
                return new JsonResult(new { });
            }
        }

    }
}
