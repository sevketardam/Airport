using Airport.DBEntities.Entities;
using System.Collections.Generic;

namespace Airport.UI.Models.VM;

public class DashboardVM
{
    public List<MyCars> MyCars { get; set; }
    public List<Locations> MyLocations { get; set; }
    public UserDatas User { get; set; }
    public List<Reservations> Reservations { get; set; }
    public List<Reservations> AWeekReservations { get; set; }

}
