﻿using System.Collections.Generic;

namespace Airport.UI.Models.VM
{
    public class ReservationStepTwoVM
    {
        public List<GetReservationValues> ReservationValues { get; set; }
        public string PickLocationLatLng { get; set; }
        public string DropLocationLatLng { get; set; }
        public string DropLocationPlaceId { get; set; }
        public string PickLocationPlaceId { get; set; }
    }
}
