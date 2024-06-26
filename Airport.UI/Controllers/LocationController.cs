using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Airport.UI.Models.VM;
using Airport.DBEntitiesDAL.Interfaces;
using System.Linq;
using System.Security.Claims;
using Airport.UI.Models.IM;
using System.Collections.Generic;
using Airport.DBEntities.Entities;
using Airport.UI.Models.Interface;

namespace Airport.UI.Controllers;

public class LocationController(IMyCarsDAL myCarsDal, ICarBrandsDAL carBrandsDal, IGetCarDetail carDetailDal, ILocationsDAL locationsDal, ILocationCarsDAL locationCarsDal, ILocationCarsFareDAL locationCarsFareDal, IReservationsDAL reservationsDal, ICarModelsDAL carModelsDal) : PanelAuthController
{

    [HttpGet("panel/location")]
    public IActionResult Index()
    {
        var userId = Convert.ToInt32(Request.HttpContext.User.Claims.Where(a => a.Type == ClaimTypes.Sid).Select(a => a.Value).SingleOrDefault());
        var myLocation = locationsDal.SelectByFunc(a => a.UserId == userId && !a.IsDelete);
        return View(myLocation);
    }

    [HttpGet("panel/add-location/step-one")]
    public IActionResult AddLocationStepOne() => View();

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

            var myCarsList = myCarsDal.SelectByFunc(a => a.UserId == userId && !a.IsDelete);
            myCarsList.ForEach(a =>
            {
                a.Brand = carBrandsDal.SelectByID(a.BrandId);
                a.Model = carModelsDal.SelectByID(a.ModelId);
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
            result.Result.Geometry.Location.lat = convertMapValues.LocationLat;
            result.Result.Geometry.Location.lng = convertMapValues.LocationLng;

            TempData["result"] = JsonConvert.SerializeObject(result);

            convertMapValues.Cars = carDetailDal.GetCarsDetail(convertMapValues.LocationCars);

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

            var newLocation = locationsDal.Insert(new Locations
            {
                LocationMapId = result.Result.Place_id,
                LocationName = result.Result.LocationName,
                LocationRadius = result.Result.LocationRadius,
                UserId = userId,
                DropCharge = convertData.DropCharge,
                OutZonePricePerKM = convertData.OutZonePerKmPrice,
                Lat = result.Result.Geometry.Location.lat,
                Lng = result.Result.Geometry.Location.lng,
                IsDelete = false,
                IsOkeyOut = convertData.IsOutsideWork
            });

            var carLocationFareList = new List<LocationCarsFare>();

            convertData.CarsPrice.ForEach(a =>
            {
                var addedLocationCars = locationCarsDal.Insert(new LocationCars
                {
                    CarId = a.CarId,
                    LocationId = newLocation.Id,
                });

                a.CarsPricePerKm.ForEach(car =>
                {
                    carLocationFareList.Add(new LocationCarsFare
                    {
                        LocationCarId = addedLocationCars.Id,
                        StartFrom = car.StartKm,
                        UpTo = car.UpToKm,
                        Fare = car.Price.ToString(),
                        PriceType  = car.PriceType,
                    });
                });
            });

            locationCarsFareDal.InsertRage(carLocationFareList);

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

        var location = locationsDal.SelectByFunc(a => a.Id == id && a.UserId == userId).FirstOrDefault();
        if (location == null) { return NotFound(); }



        var VM = new UpdateLocationVM()
        {
            Location = location,
            locationCars = locationCarsDal.SelectByFunc(a => a.LocationId == location.Id)
        };

        VM.locationCars.ForEach(a =>
        {
            a.Car = carDetailDal.CarDetail(a.CarId);
            a.LocationCarsFares = locationCarsFareDal.SelectByFunc(b => b.LocationCarId == a.Id);
            a.LocationCarsFares.ForEach(b =>
            {
                b.LocationCar = locationCarsDal.SelectByID(a.Id);
                b.LocationCar.Car = carDetailDal.CarDetail(a.CarId);
            });
        });

        return View(VM);
    }

    [HttpPost("panel/update-location/{id}", Name = "updateLocationValues")]
    public IActionResult UpdateLocation(string jsonValues)
    {

        //!!!!!!!BURASI DÜZENLENİCEK HATALI SİLİNİNCE IDLER DEĞİŞİYOR BU YÜZDEN RESERVATION DETAYI HATALI GELİYOR
        var userId = Convert.ToInt32(Request.HttpContext.User.Claims.Where(a => a.Type == ClaimTypes.Sid).Select(a => a.Value).SingleOrDefault());
        var convertData = JsonConvert.DeserializeObject<GetLocationDataVM>(jsonValues);

        var location = locationsDal.SelectByFunc(a => a.Id == convertData.LocationId && a.UserId == userId).FirstOrDefault();

        if (location == null) { return BadRequest(); }

        location.OutZonePricePerKM = convertData.OutZonePerKmPrice;
        location.DropCharge = convertData.DropCharge;
        location.LocationName = convertData.LocationName;
        location.LocationRadius = convertData.LocationZone;
        location.IsOkeyOut = convertData.IsOutsideWork;
        locationsDal.Update(location);

        var oldLocationCar = locationCarsDal.SelectByFunc(a => a.LocationId == location.Id);
        oldLocationCar.ForEach(a =>
        {
            var oldLocationCarFare = locationCarsFareDal.SelectByFunc(b => b.LocationCarId == a.Id);
            oldLocationCarFare.ForEach(b =>
            {
                locationCarsFareDal.HardDelete(b);
            });

            //_locationCars.HardDelete(a);
        });

        var carLocationFareList = new List<LocationCarsFare>();

        convertData.CarsPrice.ForEach(a =>
        {
            //var addedLocationCars = _locationCars.Insert(new LocationCars
            //{
            //    CarId = a.CarId,
            //    LocationId = location.Id,
            //});

            a.CarsPricePerKm.ForEach(car =>
            {
                carLocationFareList.Add(new LocationCarsFare
                {
                    LocationCarId = a.CarId,
                    StartFrom = car.StartKm,
                    UpTo = car.UpToKm,
                    Fare = car.Price.ToString(),
                    PriceType = car.PriceType
                });
            });
        });

        locationCarsFareDal.InsertRage(carLocationFareList);

        return RedirectToAction("Index", "Location");
    }

    public IActionResult DeleteLocation(int id)
    {
        var userId = Convert.ToInt32(Request.HttpContext.User.Claims.Where(a => a.Type == ClaimTypes.Sid).Select(a => a.Value).SingleOrDefault());
        var location = locationsDal.SelectByFunc(a=>a.Id == id && a.UserId == userId).FirstOrDefault();
        if (location == null)
        {
            return Json(new { result = 2 });
        }

        //var check = false;
        //var locationCars = _locationCars.SelectByFunc(a=>a.LocationId == location.Id);
        //locationCars.ForEach(a =>
        //{
        //    var checkReservation = _reservations.SelectByFunc(b=>b.LocationCarId == a.Id).FirstOrDefault();
        //    if (checkReservation is not null)
        //    {
        //        check = true;
        //        //var locationCarsFare = _locationCarsFare.SelectByFunc(b => b.LocationCarId == a.Id);
        //        //locationCarsFare.ForEach(b =>
        //        //{
        //        //    _locationCarsFare.HardDelete(b);
        //        //});

        //        //_locationCars.HardDelete(a);
        //    }
        //    //else
        //    //{
        //    //    check = true;
        //    //}
        //});

        //if (check)
        //{
            locationsDal.SoftDelete(location);
        //}
        //else
        //{
        //    _locations.HardDelete(location);
        //}


        return Json(new { result = 1 });
    }

}