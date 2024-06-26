using Airport.Data;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Airport.DBEntities.Entities;

public class LocationCars : IEntity
{
    [Key]
    public int Id { get; set; }
    public int LocationId { get; set; }
    public double Price { get; set; }
    public int CarId { get; set; }

    public Locations Location { get; set; }
    public List<LocationCarsFare> LocationCarsFares { get; set; }
    public List<Reservations> Reservations { get; set; }
    public MyCars Car { get; set; }
}
