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
        public double Price { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string DropPlaceId { get; set; }
        public string PickPlaceId { get; set; }
        public int LocationCarId { get; set; }
        public string ReservationCode { get; set; }


        public LocationCars LocationCars { get; set; }
    }
}
