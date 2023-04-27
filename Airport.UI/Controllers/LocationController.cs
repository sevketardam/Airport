using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Airport.UI.Models.VM;
using Microsoft.AspNetCore.Routing;
using Airport.DBEntitiesDAL.Interfaces;

namespace Airport.UI.Controllers
{
    public class LocationController : Controller
    {
        IMyCarsDAL _myCars;
        public LocationController(IMyCarsDAL myCars)
        {
            _myCars = myCars;
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
            string apiKey = "AIzaSyAnqSEVlrvgHJymL-F8GmxIwNbe8fYUjdg";

            var httpClient = new HttpClient();
            var apiUrl = "https://maps.googleapis.com/maps/api/place/details/json?place_id=" + placeId + "&key=" + apiKey;
            var response = await httpClient.GetAsync(apiUrl);
            var content = await response.Content.ReadAsStringAsync();
            var contentJsonResult = JsonConvert.DeserializeObject<GetGoogleAPIVM>(content);

            if (contentJsonResult.Status == "OK")
            {
                return RedirectToAction("AddLocationStepTwo", "Location", new { status = contentJsonResult.Status, resultModel = JsonConvert.SerializeObject(contentJsonResult.Result)});
            }
            return View();
        }


        [HttpGet("panel/add-location/step-tow")]
        public IActionResult AddLocationStepTwo(GetGoogleAPIVM model,string resultModel)
        {
            var convertResult = JsonConvert.DeserializeObject<GoogleAPIResultVM>(resultModel);
            model.Result = new GoogleAPIResultVM
            {
                Place_id = convertResult.Place_id,
                Geometry = convertResult.Geometry,
            };
            ViewBag.Location = "{ lat: " + model.Result.Geometry.Location.lat + ", lng:" + model.Result.Geometry.Location.lng + "}";




            return View();
        }

    }
}
