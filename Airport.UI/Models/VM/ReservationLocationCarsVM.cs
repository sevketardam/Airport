using Airport.DBEntities.Entities;

namespace Airport.UI.Models.VM
{
    public class ReservationLocationCarsVM 
    {
        public LocationCars LocationCar { get; set; }
        public string PlaceId { get; set; }
        public string ZoneValue { get; set; }
        public string Lat { get; set; }
        public string Lng { get; set; }
    }
}
