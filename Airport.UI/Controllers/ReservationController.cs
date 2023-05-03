using Airport.DBEntities.Entities;
using Airport.DBEntitiesDAL.Interfaces;
using Airport.MessageExtensions.Interfaces;
using Airport.UI.Models.Extendions;
using Airport.UI.Models.IM;
using Airport.UI.Models.Interface;
using Airport.UI.Models.VM;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Airport.MessageExtension.VM;
using static System.Net.WebRequestMethods;
using Microsoft.CodeAnalysis;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Asn1.Cms;
using Microsoft.AspNetCore.Authorization;

namespace Airport.UI.Controllers
{
    public class ReservationController : Controller
    {




        ILocationsDAL _location;
        ILocationCarsDAL _locationCar;
        ILocationCarsFareDAL _locationCarsFare;
        IGetCarDetail _carDetail;
        IUserDatasDAL _userDatas;
        IReservationsDAL _reservations;
        IGetCarDetail _getCar;
        IReservationPeopleDAL _reservationsPeople;
        IMail _mail;

        public ReservationController(ILocationsDAL location, ILocationCarsDAL locationCar, ILocationCarsFareDAL locationCarsFare, IGetCarDetail carDetail, IUserDatasDAL userDatas, IReservationsDAL reservations, IGetCarDetail getCar, IReservationPeopleDAL reservationsPeople, IMail mail)
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
                    var locations = _location.Select();
                    var listlocation = new List<ReservationLocationCarsVM>();
                    var locationCars = new List<List<ReservationLocationCarsVM>>();
                    int i = 0;

                    locations.ForEach(a =>
                    {
                        a.LocationCars = _locationCar.SelectByFunc(b => b.LocationId == a.Id);
                        a.LocationCars.ForEach(b =>
                        {

                            listlocation.Add(new ReservationLocationCarsVM
                            {
                                LocationCar = b,
                                PlaceId = a.LocationMapId,
                                ZoneValue = a.LocationRadius,
                                Lat = a.Lat,
                                Lng = a.Lng
                            });

                            i++;
                            if (i == 25)
                            {
                                locationCars.Add(listlocation);

                                listlocation = new List<ReservationLocationCarsVM>();
                                i = 0;
                            }
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
                        var convertLocation = _location.SelectByFunc(b => b.Lat == a.Lat && b.Lng == a.Lng);
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
                    int minKm = 0;
                    var lastKm = Math.Ceiling(Convert.ToDouble(betweenLocation.rows[0].elements[0].distance.value) / 1000) * 1000;
                    minKm = Convert.ToInt32(lastKm / 1000);
                    var selectedLocationsMini = new List<LocationIsOutMiniVM>();
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

                            double price = 0;
                            if (a.IsOutZone)
                            {
                                price = a.Location.DropCharge + (minKm * a.Location.OutZonePricePerKM);

                                if (reservation.ReturnStatus)
                                {
                                    price *= 2;
                                }

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
                            else
                            {
                                b.LocationCarsFares = _locationCarsFare.SelectByFunc(c => c.LocationCarId == b.Id);
                                var lastUp = 0;
                                double lastPrice = 0;

                                b.LocationCarsFares.ForEach(c =>
                                {

                                    if (c.UpTo < minKm)
                                    {
                                        if (c.PriceType == 1)
                                        {
                                            price += c.Fare * (c.UpTo - c.StartFrom);
                                        }
                                        else
                                        {
                                            price += c.Fare;
                                        }
                                    }

                                    lastUp = c.UpTo;
                                    lastPrice = c.Fare;
                                });


                                if (lastUp < minKm)
                                {
                                    var plusPrice = minKm - lastUp;
                                    price += lastPrice * plusPrice;
                                }

                                if (reservation.ReturnStatus)
                                {
                                    price *= 2;
                                }

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


                    var lastVM = new ReservationStepTwoVM()
                    {
                        ReservationValues = getreservation,
                        DropLocationLatLng = $"lat:{contentJsonResult.Result.Geometry.Location.lat},lng:{contentJsonResult.Result.Geometry.Location.lng}",
                        PickLocationLatLng = $"lat:{contentJsonResult2.Result.Geometry.Location.lat},lng:{contentJsonResult2.Result.Geometry.Location.lng}",
                        DropLocationPlaceId = reservation.DropValue,
                        PickLocationPlaceId = reservation.PickValue,
                        Distance = betweenLocation.rows[0].elements[0].distance.text,
                        Duration = betweenLocation.rows[0].elements[0].duration.text
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
                        Duration = betweenLocation.rows[0].elements[0].duration.text
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

        [HttpGet("reservation-step-three/{id}")]
        public async Task<IActionResult> ReservationStepThree(int id)
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

                        if (datas.ReservationValues.ReturnStatus)
                        {
                            price *= 2;
                        }
                    }
                    else
                    {
                        locationCar.LocationCarsFares = _locationCarsFare.SelectByFunc(c => c.LocationCarId == locationCar.Id);

                        locationCar.LocationCarsFares.ForEach(c =>
                        {

                            if (c.UpTo < datas.KM)
                            {
                                if (c.PriceType == 1)
                                {
                                    price += c.Fare * (c.UpTo - c.StartFrom);
                                }
                                else
                                {
                                    price += c.Fare;
                                }
                            }

                            lastUp = c.UpTo;
                            lastPrice = c.Fare;
                        });


                        if (lastUp < datas.KM)
                        {
                            var plusPrice = datas.KM - lastUp;
                            price += lastPrice * plusPrice;
                        }

                        if (datas.ReservationValues.ReturnStatus)
                        {
                            price *= 2;
                        }

                    }

                    datas.LastPrice = price;
                    datas.LocationCar = locationCar;
                    datas.LocationCar.Car = _carDetail.CarDetail(locationCar.CarId);

                    HttpContext.Session.Remove("reservationData");

                    HttpContext.Session.MySet("reservationData", datas);

                    var reservation = new ReservationStepThreeVM()
                    {
                        SelectedData = datas,
                        User = user
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
        public async Task<IActionResult> ReservationLastStep(Reservations reservation, List<string> OthersName, List<string> OthersSurname)
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
                    Price = createReservation.LastPrice,
                    Surname = reservation.Surname,
                    DropFullName = createReservation.DropLocationName,
                    PickFullName = createReservation.PickLocationName,
                    PeopleCount = createReservation.ReservationValues.PeopleCount,
                    ReservationDate = createReservation.ReservationValues.FlightTime,
                    ReturnDate = createReservation.ReservationValues.ReturnDate,
                    ReturnStatus = createReservation.ReservationValues.ReturnStatus,
                    DistanceText = createReservation.Distance,
                    DurationText = createReservation.Duration,
                });

                item.LocationCars = _locationCar.SelectByID(item.LocationCarId);
                item.LocationCars.Car = _getCar.CarDetail(item.LocationCars.CarId);

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

                _mail.SendReservationMail(new ReservationMailVM
                {
                    Name = reservation.Name,
                    Phone = reservation.Phone,
                    ReservationCode = kod,
                    Surname = reservation.Surname,
                    Email = reservation.Email,
                    Price = createReservation.LastPrice.ToString()
                });

                HttpContext.Session.Remove("reservationData");

                return View(item);
            }
            catch (Exception)
            {
                ViewBag.Error = "Error";
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


        [Authorize(Roles = "0")]
        [HttpGet("panel/manual-reservation-one",Name ="getManualLocationValue")]
        public async Task<IActionResult> ManualReservationStepOne(GetResevationIM reservation)
        {

            return View();
        }
        [Authorize(Roles = "0")]
        [HttpGet("panel/manual-reservation-two")]
        public IActionResult ManualReservationStepTwo()
        {
            return View();
        }
        [Authorize(Roles = "0")]
        [HttpGet("panel/manual-reservation-three")]
        public IActionResult ManualReservationStepThree()
        {
            return View();
        }






    }
}
