using System.Collections.Generic;

namespace Airport.UI.Models.VM;

public class GetReservationListVM
{
    public List<LocationIsOutMiniVM> MiniLocations { get; set; }
    public List<GetReservationValues> Locations { get; set; }
}
