using Airport.DBEntities.Entities;
using Airport.UI.Models.IM;
using System.Collections.Generic;

namespace Airport.UI.Models.VM
{
    public class ReservationStepTwoVM
    {
        public List<GetReservationValues> ReservationValues { get; set; }
        public string PickLocationLatLng { get; set; }
        public string DropLocationLatLng { get; set; }
        public string DropLocationPlaceId { get; set; }
        public string PickLocationPlaceId { get; set; }
        public string Distance { get; set; }
        public string Duration { get; set; }
        public GetResevationIM SelectedReservationValues { get; set; }
        public Reservations UpdateReservationValues { get; set; }
    }
}
