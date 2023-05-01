using Airport.DBEntities.Entities;
using Airport.DBEntitiesDAL.Interfaces;
using Airport.UI.Models.IM;
using Airport.UI.Models.Interface;
using Airport.UI.Models.VM;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection.Metadata;
using System.Security.Claims;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

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

        public ReservationController(ILocationsDAL location, ILocationCarsDAL locationCar, ILocationCarsFareDAL locationCarsFare, IGetCarDetail carDetail, IUserDatasDAL userDatas, IReservationsDAL reservations,IGetCarDetail getCar)
        {
            _location = location;
            _locationCar = locationCar;
            _locationCarsFare = locationCarsFare;
            _carDetail = carDetail;
            _userDatas = userDatas;
            _reservations = reservations;
            _getCar = getCar;
        }

        [HttpPost("reservation", Name = "getLocationValue")]
        public async Task<IActionResult> ReservationStepTwo(GetResevationIM reservation)
        {
            try
            {
                var api_key = "AIzaSyAnqSEVlrvgHJymL-F8GmxIwNbe8fYUjdg";

                var httpClient = new HttpClient();

                var apiUrl = "https://maps.googleapis.com/maps/api/place/details/json?place_id=" + reservation.PickValue + "&key=" + api_key;
                var response2 = await httpClient.GetAsync(apiUrl);
                var content2 = await response2.Content.ReadAsStringAsync();
                var contentJsonResult = JsonConvert.DeserializeObject<GetGoogleAPIVM>(content2);

                var apiUrl2 = "https://maps.googleapis.com/maps/api/place/details/json?place_id=" + reservation.DropValue + "&key=" + api_key;
                var response3 = await httpClient.GetAsync(apiUrl2);
                var content3 = await response3.Content.ReadAsStringAsync();
                var contentJsonResult2 = JsonConvert.DeserializeObject<GetGoogleAPIVM>(content3);


                var s = "https://maps.googleapis.com/maps/api/distancematrix/json?units=metric";

                var fullUrl2 = s + $"&origins={contentJsonResult.Result.Geometry.Location.lat},{contentJsonResult.Result.Geometry.Location.lng}&destinations={contentJsonResult2.Result.Geometry.Location.lat},{contentJsonResult2.Result.Geometry.Location.lng}&key=" + api_key;
                var getreservation = new List<GetReservationValues>();
                using (var client = new HttpClient())
                {
                    HttpResponseMessage response4 = await client.GetAsync(fullUrl2);
                    var content4 = await response4.Content.ReadAsStringAsync();
                    var data = JObject.Parse(content4);

                    if (data["status"].ToString() == "OK")
                    {
                        if (data["rows"][0]["elements"][0]["status"].ToString() == "OK")
                        {
                            var km = data["rows"][0]["elements"][0]["distance"]["value"].ToString().Replace("{", "").Replace("}", "");
                            var lastKm = Math.Ceiling(Convert.ToDouble(km) / 1000) * 1000;
                            var minKm = lastKm / 1000;

                            var carLocation = _location.Select();
                            foreach (var item in carLocation)
                            {
                                item.LocationCars = _locationCar.SelectByFunc(a => a.LocationId == item.Id);

                                foreach (var item1 in item.LocationCars)
                                {
                                    var price = item1.DropPrice;
                                    item1.Car = _carDetail.CarDetail(item1.CarId);

                                    item1.LocationCarsFares = _locationCarsFare.SelectByFunc(a => a.LocationCarId == item1.Id);


                                    double lastPrice = 0;
                                    item1.LocationCarsFares.ForEach(a =>
                                    {
                                        if (minKm >= a.UpTo)
                                        {
                                            lastPrice = a.Fare;
                                            price += a.Fare * minKm;
                                        }
                                        else
                                        {
                                            if (a.StartFrom == 0)
                                            {
                                                lastPrice = a.Fare;
                                                price += a.Fare * minKm;
                                            }
                                            else
                                            {
                                                price += lastPrice * minKm;
                                            }

                                        }

                                    });

                                    getreservation.Add(new GetReservationValues
                                    {
                                        LocationCars = item1,
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
                            }
                        }
                    }
                }

                var lastVM = new ReservationStepTwoVM()
                {
                    ReservationValues = getreservation,
                    DropLocationLatLng = $"lat:{contentJsonResult.Result.Geometry.Location.lat},lng:{contentJsonResult.Result.Geometry.Location.lng}",
                    PickLocationLatLng = $"lat:{contentJsonResult2.Result.Geometry.Location.lat},lng:{contentJsonResult2.Result.Geometry.Location.lng}",
                    DropLocationPlaceId = reservation.DropValue,
                    PickLocationPlaceId = reservation.PickValue,
                };

                TempData["datas"] = JsonConvert.SerializeObject(getreservation);

                return View(lastVM);
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

                var datas = JsonConvert.DeserializeObject<List<GetReservationValues>>((string)TempData["datas"]);
                var selectedData = datas.Where(a => a.LocationCars.Id == id).FirstOrDefault();

                TempData["createReservation"] = JsonConvert.SerializeObject(selectedData);

                var reservation = new ReservationStepThreeVM()
                {
                    SelectedData = selectedData,
                    User = user
                };

                return View(reservation);
            }
            catch (Exception)
            {
                ViewBag.Error = "Error";
                return RedirectToAction("Index", "Home");
            }

        }

        [HttpPost("reservation-get-code", Name = "getBookValues")]
        public async Task<IActionResult> ReservationLastStep(Reservations reservation)
        {
            try
            {
                var createReservation = JsonConvert.DeserializeObject<GetReservationValues>((string)TempData["createReservation"]);

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
                    LocationCarId = createReservation.LocationCars.Id,
                    Name = reservation.Name,
                    ReservationCode = kod,
                    Price = createReservation.LastPrice,
                    Surname = reservation.Surname,
                    DropFullName = createReservation.DropLocationName,
                    PickFullName = createReservation.PickLocationName,
                    PeopleCount = createReservation.PassangerCount,
                    ReservationDate = createReservation.ReservationDate,
                });

                item.LocationCars = _locationCar.SelectByID(item.LocationCarId);
                item.LocationCars.Car = _getCar.CarDetail(item.LocationCars.CarId);

                return View(item);
            }
            catch (Exception)
            {
                ViewBag.Error = "Error";
                return RedirectToAction("Index", "Home");
            }

        }


        [HttpPost("manage-reservation",Name = "checkReservation")]
        public async Task<IActionResult> GetReservation(string reservationCode, string email)
        {
            try
            {
                var reservation = _reservations.SelectByFunc(a => a.Email == email && a.ReservationCode == reservationCode).FirstOrDefault();
                if (reservation == null)
                {
                    ViewBag.Warning = "Warning";
                    return RedirectToAction("ManageReservation","Home");
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

    }
}
