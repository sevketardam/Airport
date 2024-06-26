using Airport.DBEntities.Entities;
using Airport.UI.Models.Interface;
using Airport.UI.Models.VM;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace Airport.UI.Models.ITransactions;

public class ApiResults(IOptions<GoogleAPIKeys> googleAPIKeys) : IApiResult
{
    public async Task<DistanceMatrixApiResponse> DistanceMatrixValues(string pickLat, string pickLng, string dropLat, string dropLng)
    {
        var httpClient = new HttpClient();
        var api_key = googleAPIKeys.Value.GoogleMapAPIKey;

        var fullUrl2 =$"https://maps.googleapis.com/maps/api/distancematrix/json?units=metric&origins={pickLat},{pickLng}&destinations={dropLat},{dropLng}&key=" + api_key;
        var response4 = await httpClient.GetAsync(fullUrl2);
        var content4 = await response4.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<DistanceMatrixApiResponse>(content4);
    }

    public async Task<DistanceMatrixApiResponse> DistanceMatrixValues(string pickLat, string pickLng, string carLatLngString)
    {
        var httpClient = new HttpClient();
        var api_key = googleAPIKeys.Value.GoogleMapAPIKey;

        var fullUrl2 = $"https://maps.googleapis.com/maps/api/distancematrix/json?units=metric&origins={pickLat},{pickLng}&destinations={carLatLngString}&key=" + api_key;
        var response4 = await httpClient.GetAsync(fullUrl2);
        var content4 = await response4.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<DistanceMatrixApiResponse>(content4);
    }

    public async Task<GetGoogleAPIVM> LocationValues(string placeId)
    {
        var api_key = googleAPIKeys.Value.GoogleMapAPIKey;

        var httpClient = new HttpClient();
        var apiUrl = "https://maps.googleapis.com/maps/api/place/details/json?place_id=" + placeId + "&key=" + api_key;
        var response = await httpClient.GetAsync(apiUrl);
        var content = await response.Content.ReadAsStringAsync();

        return JsonConvert.DeserializeObject<GetGoogleAPIVM>(content);
    }
}
