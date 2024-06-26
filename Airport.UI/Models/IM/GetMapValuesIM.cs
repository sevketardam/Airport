using Airport.DBEntities.Entities;
using System.Collections.Generic;

namespace Airport.UI.Models.IM;

public class GetMapValuesIM
{
    public string LocationName { get; set; }
    public string LocationRadius { get; set; }
    public string LocationLat { get; set; }
    public string LocationLng { get; set; }
    public int[] LocationCars { get; set; }


    public List<MyCars> Cars { get; set; }
}
