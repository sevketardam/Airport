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

namespace Airport.UI.Controllers
{
    public class LocationController : BaseController
    {
        IMyCarsDAL _myCars;
        ICarBrandsDAL _myBrands;
        IGetCarDetail _carDetail;
        public LocationController(IMyCarsDAL myCars, ICarBrandsDAL carBrands, IGetCarDetail carDetail)
        {
            _myCars = myCars;
            _myBrands = carBrands;
            _carDetail = carDetail;
        }


        [HttpGet("panel/location")]
        public IActionResult Index()
        {
            return View();
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

            var convertData = JsonConvert.DeserializeObject<GetLocationDataVM>(jsonValues);

            return View();
        }

    }
}
