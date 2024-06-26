using Airport.Data;
using System.ComponentModel.DataAnnotations;

namespace Airport.DBEntities.Entities;

public class LocationCarsFare : IEntity
{
    [Key]
    public int Id { get; set; }
    public int StartFrom { get; set; }
    public int UpTo { get; set; }
    public string Fare { get; set; }
    public byte PriceType { get; set; }
    public int LocationCarId { get; set; }
    public bool IsDelete { get; set; }
    public LocationCars LocationCar { get; set; }
}
