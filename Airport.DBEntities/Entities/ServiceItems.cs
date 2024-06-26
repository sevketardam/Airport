using Airport.Data;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Airport.DBEntities.Entities;

public class ServiceItems : IEntity
{
    [Key]
    public int Id { get; set; }
    public int ServiceId { get; set; }
    public int ServicePropertyId { get; set; }
    public double Price { get; set; }


    public Services Service { get; set; }
    public List<ReservationServicesTable> ReservationServicesTables { get; set; }
    public ServiceProperties ServiceProperty { get; set; }
}
