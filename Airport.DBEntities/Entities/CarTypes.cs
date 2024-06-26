using Airport.Data;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Airport.DBEntities.Entities;

public class CarTypes : IEntity
{
    [Key]
    public int Id { get; set; }
    public string CarTypeName { get; set; }
    public string CarImageURL { get; set; }

    public List<MyCars> MyCars { get; set; }
}
