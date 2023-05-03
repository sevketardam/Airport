using Airport.DBEntities.Entities;
using System.Collections.Generic;

namespace Airport.UI.Models.VM
{
    public class LocationCarsPriceVM
    {
        public List<ReservationLocationCarsVM> LocationCars { get; set; }
        public string PlaceId { get; set; }
        public string ZoneValue { get; set; }
        public string Lat { get; set; }
        public string Lng { get; set; }
    }
}
