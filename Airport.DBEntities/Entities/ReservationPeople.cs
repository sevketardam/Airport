using Airport.Data;

namespace Airport.DBEntities.Entities;

public class ReservationPeople : IEntity
{
    public int Id { get; set; } 
    public int ReservationId { get; set; } 
    public string Name { get; set; } 
    public string Surname { get; set; }


    public Reservations Reservation { get; set; }
}
