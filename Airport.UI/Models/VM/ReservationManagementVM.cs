using Airport.DBEntities.Entities;
using System.Collections.Generic;

namespace Airport.UI.Models.VM
{
    public class ReservationManagementVM
    {
        public Reservations Reservation { get; set; }
        public List<Drivers> Drivers { get; set; }
        public List<ReservationServicesTable> ReservationServicesTable { get; set; }
    }
}
