using Airport.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Airport.DBEntities.Entities
{
    public class ReservationPeople : IEntity
    {
        public int Id { get; set; } 
        public int ReservationId { get; set; } 
        public string Name { get; set; } 
        public string Surname { get; set; }


        public Reservations Reservation { get; set; }
    }
}
