using Airport.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Airport.DBEntities.Entities
{
    public class Reservations : IEntity
    {
        public int Id { get; set; }
        public string DropLatLng { get; set; }
        public string PickLatLng { get; set; }
        public double OfferPrice { get; set; }
        public string Name { get; set; }
        public string? Surname { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string DropPlaceId { get; set; }
        public string PickPlaceId { get; set; }
        public int LocationCarId { get; set; }
        public string ReservationCode { get; set; }
        public string DropFullName { get; set; }
        public string PickFullName { get; set; }
        public int PeopleCount { get; set; }
        public DateTime ReservationDate { get; set; }
        public bool ReturnStatus { get; set; }
        public DateTime ReturnDate { get; set; }
        public string DistanceText { get; set; }
        public string DurationText { get; set; }
        public bool IsDiscount { get; set; }
        public double? Discount { get; set; }
        public int UserId { get; set; }
        public string Comment { get; set; }
        public double? ServiceFee { get; set; }

        public LocationCars LocationCars { get; set; }
        public UserDatas User { get; set; }
        public List<ReservationPeople> ReservationPeoples { get; set; }
    }
}
