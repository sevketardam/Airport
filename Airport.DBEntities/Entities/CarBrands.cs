using Airport.Data;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Airport.DBEntities.Entities;

public class CarBrands : IEntity
{
    [Key]
    public int Id { get; set; }
    public string CarBrandName { get; set; }

    public List<CarModels> CarModels { get; set; }
    public List<MyCars> MyCars { get; set; }
}
