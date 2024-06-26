using System.Collections.Generic;

namespace Airport.UI.Models.VM;

public class GetLocationDataVM
{
    public int LocationId { get; set; }
    public double DropCharge { get; set; }
    public double OutZonePerKmPrice { get; set; }
    public string LocationName { get; set; }
    public string LocationZone { get; set; }
    public bool IsOutsideWork { get; set; }
    public List<GetLocationCarsPriceVM> CarsPrice { get; set; }
}
