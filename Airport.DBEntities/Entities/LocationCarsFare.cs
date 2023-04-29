﻿using Airport.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Airport.DBEntities.Entities
{
    public class LocationCarsFare : IEntity
    {
        [Key]
        public int Id { get; set; }
        public int StartFrom { get; set; }
        public int UpTo { get; set; }
        public double Fare { get; set; }
        public int LocationCarId { get; set; }

        public LocationCars LocationCar { get; set; }
    }
}
