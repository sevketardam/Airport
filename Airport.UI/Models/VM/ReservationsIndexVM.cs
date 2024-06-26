using Airport.DBEntities.Entities;
using System.Collections.Generic;

namespace Airport.UI.Models.VM;

public class ReservationsIndexVM
{
    public List<Reservations> Reservations { get; set; }
    public List<Drivers> Drivers { get; set; }
    public int ReservationCanceledCount { get; set; }
    public int ReservationCompletedCount { get; set; }
    public int ReservationPendingCount { get; set; }
    public int ReservationTotalCount { get; set; }
}
