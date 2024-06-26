﻿using Airport.Data;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Airport.DBEntities.Entities;

public class CarSeries : IEntity
{
    [Key]
    public int Id { get; set; }
    public string CarSeriesName { get; set; }
    public int CarModelId { get; set; }
    public string CarSeriesFullName { get; set; }
    public string GenerationName { get; set; }
    public string? GenerationYearBegin { get; set; }
    public string? GenerationYearEnd { get; set; }

    public CarModels CarModel { get; set; }
    public List<MyCars> MyCars { get; set; }
}
