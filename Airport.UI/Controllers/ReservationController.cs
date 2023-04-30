using Airport.DBEntities.Entities;
using Airport.DBEntitiesDAL.Interfaces;
using Airport.UI.Models.IM;
using Airport.UI.Models.VM;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Airport.UI.Controllers
{
    public class ReservationController : Controller
    {
        ILocationsDAL _location;
        public ReservationController(ILocationsDAL location)
        {
            _location = location;
        }

        [HttpPost("reservation", Name = "getLocationValue")]
        public async Task<IActionResult> ReservationStepTwo(GetResevationIM reservation)
        {

            var api_key = "AIzaSyAnqSEVlrvgHJymL-F8GmxIwNbe8fYUjdg";

            var httpClient = new HttpClient();

            var apiUrl = "https://maps.googleapis.com/maps/api/place/details/json?place_id=" + reservation.PickValue + "&key=" + api_key;
            var radius2 = 70000;
            var response2 = await httpClient.GetAsync(apiUrl);
            var content2 = await response2.Content.ReadAsStringAsync();
            var contentJsonResult = JsonConvert.DeserializeObject<GetGoogleAPIVM>(content2);

            // API çağrısı yapılacak URL
            var url = "https://maps.googleapis.com/maps/api/place/nearbysearch/json";

            // API anahtarı

            // API çağrısı için gerekli parametreler
            var location = contentJsonResult.Result.Geometry.Location.lat+","+ contentJsonResult.Result.Geometry.Location.lng;
            var radius = "70000";
            var keyword = "coffee";

            // Tüm sonuçları toplamak için bir liste oluşturun
            var allResults = new List<JToken>();

            // API çağrısı sırasında kullanılacak parametreleri ayarlayın
            var queryParams = new Dictionary<string, string>()
            {
                { "key", api_key },
                { "location", location },
                { "radius", radius },
                { "keyword", keyword }
            };

            // Tüm sayfaları dolaşmak için bir döngü oluşturun
            string nextPageToken = null;
            do
            {
                // API çağrısında bir sonraki sayfa belirteci kullanın (varsa)
                if (nextPageToken != null)
                {
                    queryParams["pagetoken"] = nextPageToken;
                }

                // API çağrısı yapmak için parametreleri birleştirin
                var fullUrl = url + "?key=" +api_key+"&location="+location+"&radius="+radius+"&keyword="+keyword+(nextPageToken != null ? "&pagetoken="+nextPageToken : "");

                // HttpClient ile API çağrısı yapın ve yanıtı alın
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync(fullUrl);
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();

                        // Yanıtı JToken olarak ayrıştırın ve sonuçları toplayın
                        var data = JObject.Parse(content);
                        var results = data["results"];
                        allResults.AddRange(results);

                        // Bir sonraki sayfa belirteci varsa, kaydedin
                        nextPageToken = (string)data["next_page_token"];
                    }
                }

                await Task.Delay(2000);
            }
            while (nextPageToken != null);

            // Tüm sonuçları yazdırın
            foreach (var result in allResults)
            {
                var locationData = result["geometry"]["location"];
                var name = (string)result["name"];
                var latitude = (double)locationData["lat"];
                var longitude = (double)locationData["lng"];
                var address = (string)result["vicinity"];

                //Console.WriteLine("Name: {0}", name);
                //Console.WriteLine("Latitude: {0}", latitude);
                //Console.WriteLine("Longitude: {0}", longitude);
                //Console.WriteLine("Address: {0}", address);
                //Console.WriteLine();
            }





            //string apiKey = "AIzaSyAnqSEVlrvgHJymL-F8GmxIwNbe8fYUjdg";

            //var httpClient2 = new HttpClient();
            //var apiUrl = "https://maps.googleapis.com/maps/api/place/details/json?place_id=" + reservation.PickValue + "&key=" + apiKey;
            //var radius = 70000;
            //var response = await httpClient.GetAsync(apiUrl);
            //var content = await response.Content.ReadAsStringAsync();
            //var contentJsonResult = JsonConvert.DeserializeObject<GetGoogleAPIVM>(content);

            //string nextPageToken = null;
            //var resultList = new List<Result>();
            //var resultList2 = new List<Result>();

            //if (contentJsonResult.Status == "OK")
            //{
            //    do
            //    {
            //        var apiUrl2 = "https://maps.googleapis.com/maps/api/place/nearbysearch/json?key=" + apiKey + "&location=" + contentJsonResult.Result.Geometry.Location.lat + "," + contentJsonResult.Result.Geometry.Location.lng + "&radius=" + radius + (nextPageToken != "" ? "&pagetoken=" + nextPageToken : "");
            //        var response2 = await httpClient.GetAsync(apiUrl2);
            //        var content2 = await response2.Content.ReadAsStringAsync();
            //        var contentJsonResult2 = JsonConvert.DeserializeObject<PlacesApiResponse>(content2);

            //        resultList.AddRange(contentJsonResult2.results);
            //        foreach (var item in contentJsonResult2.results)
            //        {
            //            resultList.Add(item);
            //        }

            //        nextPageToken = contentJsonResult2.next_page_token;

            //    } while (nextPageToken != null);
            //}



            //foreach (var a in resultList)
            //{
            //    resultList2.Add(a);
            //    do
            //    {
            //        var apiUrl3 = "https://maps.googleapis.com/maps/api/place/nearbysearch/json?key=" + apiKey + "&location=" + a.geometry.location.lat + "," + a.geometry.location.lng + "&radius=10000" + (nextPageToken != "" && nextPageToken != null ? "&pagetoken=" + nextPageToken : "");

            //        var response3 = await httpClient2.GetAsync(apiUrl3);
            //        var content3 = await response3.Content.ReadAsStringAsync();
            //        var contentJsonResult3 = JsonConvert.DeserializeObject<PlacesApiResponse>(content3);

            //        nextPageToken = contentJsonResult3.next_page_token;

            //        foreach (var item2 in contentJsonResult3.results)
            //        {
            //            resultList2.Add(item2);
            //        }

            //    } while (nextPageToken != null);
            //}

            //var locations = _location.Select();

            //var matchingLocations = new List<Locations>();

            //foreach (var location1 in locations)
            //{
            //    foreach (var location2 in resultList)
            //    {
            //        if (location1.LocationMapId == location2.place_id)
            //        {
            //            matchingLocations.Add(location1);
            //        }
            //    }
            //}



            return View(contentJsonResult);
        }
    }

    public class Location
    {
        public double lat { get; set; }
        public double lng { get; set; }
    }

    public class Geometry
    {
        public Location location { get; set; }
    }

    public class Result
    {
        public Geometry geometry { get; set; }
        public string icon { get; set; }
        public string name { get; set; }
        public string place_id { get; set; }
        public string vicinity { get; set; }
        public double rating { get; set; }
        public List<string> types { get; set; }
    }

    public class PlacesApiResponse
    {
        public List<Result> results { get; set; }
        public string next_page_token { get; set; }
    }
}
