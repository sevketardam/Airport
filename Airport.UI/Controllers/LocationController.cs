using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Airport.UI.Models.VM;
using Microsoft.AspNetCore.Routing;
using Airport.DBEntitiesDAL.Interfaces;
using System.Linq;
using System.Security.Claims;
using Airport.UI.Models.IM;
using System.Collections.Generic;
using Airport.DBEntities.Entities;
using Airport.UI.Models.Interface;
using System.Runtime.ConstrainedExecution;

namespace Airport.UI.Controllers
{
    public class LocationController : PanelAuthController
    {
        IMyCarsDAL _myCars;
        ICarBrandsDAL _myBrands;
        IGetCarDetail _carDetail;
        ILocationsDAL _locations;
        ILocationCarsDAL _locationCars;
        ILocationCarsFareDAL _locationCarsFare;
        public LocationController(IMyCarsDAL myCars, ICarBrandsDAL carBrands, IGetCarDetail carDetail, ILocationsDAL locations, ILocationCarsDAL locationCars, ILocationCarsFareDAL locationCarsFare)
        {
            _myCars = myCars;
            _myBrands = carBrands;
            _carDetail = carDetail;
            _locations = locations;
            _locationCars = locationCars;
            _locationCarsFare = locationCarsFare;
        }

        [HttpGet("panel/location")]
        public IActionResult Index()
        {
            var userId = Convert.ToInt32(Request.HttpContext.User.Claims.Where(a => a.Type == ClaimTypes.Sid).Select(a => a.Value).SingleOrDefault());
            var myLocation = _locations.SelectByFunc(a => a.UserId == userId);
            return View(myLocation);
        }

        [HttpGet("panel/add-location/step-one")]
        public IActionResult AddLocationStepOne()
        {
            return View();
        }

        [HttpPost("panel/add-location/step-one", Name = "AddLocationStepOne")]
        public async Task<IActionResult> AddLocationStepOne(string placeId)
        {
            try
            {
                string apiKey = "AIzaSyAnqSEVlrvgHJymL-F8GmxIwNbe8fYUjdg";

                var httpClient = new HttpClient();
                var apiUrl = "https://maps.googleapis.com/maps/api/place/details/json?place_id=" + placeId + "&key=" + apiKey;
                var response = await httpClient.GetAsync(apiUrl);
                var content = await response.Content.ReadAsStringAsync();
                var contentJsonResult = JsonConvert.DeserializeObject<GetGoogleAPIVM>(content);

                if (contentJsonResult.Status == "OK")
                {
                    return RedirectToAction("AddLocationStepTwo", "Location", new { status = contentJsonResult.Status, resultModel = JsonConvert.SerializeObject(contentJsonResult.Result) });
                }

                ViewBag.Error = "Location Not Found";
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error,Pls Try Again";
                return View();
            }

        }

        [HttpGet("panel/add-location/step-two")]
        public IActionResult AddLocationStepTwo(GetGoogleAPIVM model, string resultModel)
        {
            try
            {
                var userId = Convert.ToInt32(Request.HttpContext.User.Claims.Where(a => a.Type == ClaimTypes.Sid).Select(a => a.Value).SingleOrDefault());
                var convertResult = JsonConvert.DeserializeObject<GoogleAPIResultVM>(resultModel);
                model.Result = new GoogleAPIResultVM
                {
                    Place_id = convertResult.Place_id,
                    Geometry = convertResult.Geometry,
                };
                ViewBag.Location = "{ lat: " + model.Result.Geometry.Location.lat + ", lng:" + model.Result.Geometry.Location.lng + "}";

                TempData["modelresult"] = JsonConvert.SerializeObject(model);

                var myCarsList = _myCars.SelectByFunc(a => a.UserId == userId);
                myCarsList.ForEach(a =>
                {
                    a.Brand = _myBrands.SelectByID(a.BrandId);
                });

                return View(myCarsList);
            }
            catch (Exception)
            {
                return RedirectToAction("AddLocationStepOne", "Location");
            }

        }

        [HttpPost("panel/add-location/step-three", Name = "getMapLocation")]
        public async Task<IActionResult> AddLocationStepThree(string mapValues)
        {
            try
            {
                var convertMapValues = JsonConvert.DeserializeObject<GetMapValuesIM>(mapValues);


                var result = JsonConvert.DeserializeObject<GetGoogleAPIVM>((string)TempData["modelresult"]);

                result.Result.LocationRadius = convertMapValues.LocationRadius;
                result.Result.LocationName = convertMapValues.LocationName;


                TempData["result"] = JsonConvert.SerializeObject(result);

                convertMapValues.Cars = _carDetail.GetCarsDetail(convertMapValues.LocationCars);

                return View(convertMapValues);
            }
            catch (Exception)
            {
                return RedirectToAction("AddLocationStepTwo", "Location");
            }
        }

        [HttpPost("panel/add-location/step-four", Name = "getLocationValues")]
        public async Task<IActionResult> GetLocationValues(string jsonValues)
        {
            try
            {
                var userId = Convert.ToInt32(Request.HttpContext.User.Claims.Where(a => a.Type == ClaimTypes.Sid).Select(a => a.Value).SingleOrDefault());
                var result = JsonConvert.DeserializeObject<GetGoogleAPIVM>((string)TempData["result"]);

                var convertData = JsonConvert.DeserializeObject<GetLocationDataVM>(jsonValues);

                var newLocation = _locations.Insert(new Locations
                {
                    LocationMapId = result.Result.Place_id,
                    LocationName = result.Result.LocationName,
                    LocationRadius = result.Result.LocationRadius,
                    UserId = userId,
                    OutZoneDropCharge = convertData.OutZonePrice,
                    OutZonePricePerKM = convertData.OutZonePerKmPrice,
                    Lat = result.Result.Geometry.Location.lat,
                    Lng = result.Result.Geometry.Location.lng,
                });

                var carLocationFareList = new List<LocationCarsFare>();

                convertData.CarsPrice.ForEach(a =>
                {
                    var addedLocationCars = _locationCars.Insert(new LocationCars
                    {
                        CarId = a.CarId,
                        LocationId = newLocation.Id,
                        DropPrice = a.CarDropPrice,
                    });

                    a.CarsPricePerKm.ForEach(car =>
                    {
                        carLocationFareList.Add(new LocationCarsFare
                        {
                            LocationCarId = addedLocationCars.Id,
                            StartFrom = car.StartKm,
                            UpTo = car.UpToKm,
                            Fare = car.Price
                        });
                    });
                });

                _locationCarsFare.InsertRage(carLocationFareList);

                return RedirectToAction("Index", "Location");
            }
            catch (Exception)
            {
                ViewBag.Error = "Something is Wrong!!Try Again";
                return RedirectToAction("Index", "Location");
            }
        }

        [HttpGet("panel/update-location/{id}")]
        public IActionResult UpdateLocation(int id)
        {
            var userId = Convert.ToInt32(Request.HttpContext.User.Claims.Where(a => a.Type == ClaimTypes.Sid).Select(a => a.Value).SingleOrDefault());

            var location = _locations.SelectByFunc(a => a.Id == id && a.UserId == userId).FirstOrDefault();
            if (location == null) { return NotFound(); }

            var VM = new UpdateLocationVM()
            {
                Location = location,
                locationCars = _locationCars.SelectByFunc(a => a.LocationId == location.Id)
            };

            VM.locationCars.ForEach(a =>
            {
                a.Car = _carDetail.CarDetail(a.CarId);
                a.LocationCarsFares = _locationCarsFare.SelectByFunc(b => b.LocationCarId == a.Id);
                a.LocationCarsFares.ForEach(b =>
                {
                    b.LocationCar = _locationCars.SelectByID(a.Id);
                    b.LocationCar.Car = _carDetail.CarDetail(a.CarId);
                });
            });

            return View(VM);
        }


        [HttpPost("panel/update-location/{id}", Name = "updateLocationValues")]
        public IActionResult UpdateLocation(string jsonValues)
        {
            var userId = Convert.ToInt32(Request.HttpContext.User.Claims.Where(a => a.Type == ClaimTypes.Sid).Select(a => a.Value).SingleOrDefault());
            var convertData = JsonConvert.DeserializeObject<GetLocationDataVM>(jsonValues);

            var location = _locations.SelectByFunc(a => a.Id == convertData.LocationId && a.UserId == userId).FirstOrDefault();

            if (location == null) { return BadRequest(); }

            var oldLocationCar = _locationCars.SelectByFunc(a => a.LocationId == location.Id);
            oldLocationCar.ForEach(a =>
            {
                var oldLocationCarFare = _locationCarsFare.SelectByFunc(b=>b.LocationCarId == a.Id);
                oldLocationCarFare.ForEach(b =>
                {
                    _locationCarsFare.HardDelete(b);
                });

                _locationCars.HardDelete(a);                
            });

            var carLocationFareList = new List<LocationCarsFare>();

            convertData.CarsPrice.ForEach(a =>
            {
                var addedLocationCars = _locationCars.Insert(new LocationCars
                {
                    CarId = a.CarId,
                    LocationId = location.Id,
                    DropPrice = a.CarDropPrice,
                });

                a.CarsPricePerKm.ForEach(car =>
                {
                    carLocationFareList.Add(new LocationCarsFare
                    {
                        LocationCarId = addedLocationCars.Id,
                        StartFrom = car.StartKm,
                        UpTo = car.UpToKm,
                        Fare = car.Price
                    });
                });
            });

            _locationCarsFare.InsertRage(carLocationFareList);

            return RedirectToAction("Index", "Location");
        }

    }
}