using Airport.DBEntities.Entities;
using System.Collections.Generic;

namespace Airport.UI.Models.VM
{
    public class GetReservationValues
    {
        public LocationCars LocationCars { get; set; }
        public double LastPrice{ get; set; }
    }
}
