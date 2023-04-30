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
using System.Net.Http;
using System.Reflection.Metadata;
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

        public ReservationController(ILocationsDAL location, ILocationCarsDAL locationCar, ILocationCarsFareDAL locationCarsFare, IGetCarDetail carDetail)
        {
            _location = location;
            _locationCar = locationCar;
            _locationCarsFare = locationCarsFare;
            _carDetail = carDetail;
        }

        [HttpPost("reservation", Name = "getLocationValue")]
        public async Task<IActionResult> ReservationStepTwo(GetResevationIM reservation)
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

            using (var client = new HttpClient())
            {
                HttpResponseMessage response4 = await client.GetAsync(fullUrl2);
                var content4 = await response4.Content.ReadAsStringAsync();
                var data2 = JObject.Parse(content4);

                if (data2["status"].ToString() == "OK")
                {
                    if (data2["rows"][0]["elements"][0]["status"].ToString() == "OK")
                    {
                    }
                }
                else
                {

                }
            }

            var carLocation = _location.Select();
            var getreservation = new List<GetReservationValues>();
            using (var client = new HttpClient())
            {
                var destinationLocation = $"{contentJsonResult.Result.Geometry.Location.lat}%2C{contentJsonResult.Result.Geometry.Location.lng}";

                foreach (var item in carLocation)
                {
                    item.LocationCars = _locationCar.SelectByFunc(a => a.LocationId == item.Id);
                    var originLocation = $"{item.Lat},{item.Lng}";
                    var fullUrl = s + "&origins=" + originLocation + "&destinations=" + destinationLocation + "&key=" + api_key;
                    HttpResponseMessage response = await client.GetAsync(fullUrl);
                    var content = await response.Content.ReadAsStringAsync();
                    var data = JObject.Parse(content);
                    if (data["status"].ToString() == "OK")
                    {
                        if (data["rows"][0]["elements"][0]["status"].ToString() == "OK")
                        {
                            var km = data["rows"][0]["elements"][0]["distance"]["value"].ToString().Replace("{", "").Replace("}", "");
                            var lastKm = Math.Ceiling(Convert.ToDouble(km) / 1000) * 1000;
                            var minKm = lastKm / 1000;

                            var lastKm2 = lastKm;
                            foreach (var item1 in item.LocationCars)
                            {
                                var price = item1.DropPrice;
                                item1.Car = _carDetail.CarDetail(item1.CarId);
                                item1.LocationCarsFares = _locationCarsFare.SelectByFunc(a => a.LocationCarId == item1.Id);
                                item1.LocationCarsFares.ForEach(a =>
                                {
                                    if (minKm <= a.UpTo)
                                    {
                                        price += a.Fare;
                                    }
                                });

                                getreservation.Add(new GetReservationValues
                                {
                                    LocationCars = item1,
                                    LastPrice = price
                                });
                            }
                        }
                    }
                }
            }

            return View(getreservation);
        }
    }
}
