using Airport.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Airport.DBEntities.Entities
{
    public class ReservationServicesTable : IEntity
    {
        [Key]
        public int Id { get; set; }
        public int ReservationId { get; set; }
        public double Price { get; set; }
        public int PeopleCount { get; set; }
        public int ServiceItemId { get; set; }

        public ServiceItems ServiceItem { get; set; }
        public Reservations Reservation { get; set; }
    }
}
