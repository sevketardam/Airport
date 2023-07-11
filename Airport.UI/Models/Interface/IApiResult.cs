using Airport.UI.Models.VM;
using System.Threading.Tasks;

namespace Airport.UI.Models.Interface
{
    public interface IApiResult
    {
        Task<GetGoogleAPIVM> LocationValues(string placeId);
        Task<DistanceMatrixApiResponse> DistanceMatrixValues(string pickLat, string pickLng, string dropLat, string dropLng);
        Task<DistanceMatrixApiResponse> DistanceMatrixValues(string pickLat, string pickLng, string carLatLngString);
    }
}
