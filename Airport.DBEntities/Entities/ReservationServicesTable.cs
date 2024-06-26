using Airport.Data;
using System.ComponentModel.DataAnnotations;

namespace Airport.DBEntities.Entities;

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
