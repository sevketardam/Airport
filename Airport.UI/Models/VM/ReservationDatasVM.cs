using Airport.DBEntities.Entities;
using Airport.UI.Models.IM;

namespace Airport.UI.Models.VM
{
    public class ReservationDatasVM
    {
        public string DropLocationLatLng { get; set; }
        public string PickLocationLatLng { get; set; }
        public string DropLocationPlaceId { get; set; }
        public string PickLocationPlaceId { get; set; }
        public string DropLocationName { get; set; }
        public string PickLocationName { get; set; }
        public double KM { get; set; }
        public string Distance { get; set; }
        public string Duration { get; set; }
        public bool IsOutZone { get; set; } 
        public GetResevationIM ReservationValues { get; set; }
        public LocationCars LocationCar { get; set; }
        public Reservations UpdateReservation { get; set; }

        public double TotalPrice { get; set; }
        public double OfferPrice { get; set; }
        public double ServiceFee { get; set; }
        public double? SalesFee { get; set; }
        public double PartnerFee { get; set; }
        public double? ExtraServiceFee { get; set; }
        public double GlobalPartnerFee { get; set; }
    }
}
