using Airport.DBEntities.Entities;
using System.Collections.Generic;

namespace Airport.UI.Models.VM
{
    public class ReservationsIndexVM
    {
        public List<Reservations> Reservations { get; set; }
        public List<Drivers> Drivers { get; set; }
    }
}
