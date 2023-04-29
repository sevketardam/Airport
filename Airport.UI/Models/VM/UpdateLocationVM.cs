using Airport.DBEntities.Entities;
using System.Collections.Generic;

namespace Airport.UI.Models.VM
{
    public class UpdateLocationVM
    {
        public Locations Location { get; set; }
        public List<LocationCars> locationCars { get; set; }
    }
}
