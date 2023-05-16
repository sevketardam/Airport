using Airport.DBEntities.Entities;
using System;
using System.Collections.Generic;

namespace Airport.UI.Models.VM
{
    public class GetReservationValues
    {
        public LocationCars LocationCars { get; set; }
        public string LastPrice{ get; set; }
        public DateTime ReservationDate { get; set; }
        public string PickLocationName{ get; set; }
        public string DropLocationName{ get; set; }
        public int PassangerCount{ get; set; }
        public string PickLocationLatLng { get; set; }
        public string DropLocationLatLng { get; set; }
        public string DropLocationPlaceId { get; set; }
        public string PickLocationPlaceId { get; set; }
    }

}
