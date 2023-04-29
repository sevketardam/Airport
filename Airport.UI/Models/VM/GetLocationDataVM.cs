using System.Collections.Generic;

namespace Airport.UI.Models.VM
{
    public class GetLocationDataVM
    {
        public int LocationId { get; set; }
        public double OutZonePrice { get; set; }
        public double OutZonePerKmPrice { get; set; }
        public List<GetLocationCarsPriceVM> CarsPrice { get; set; }
    }
}
