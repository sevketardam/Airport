﻿using Airport.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Airport.DBEntities.Entities
{
    public class Locations : IEntity
    {
        [Key]
        public int Id { get; set; }
        public string LocationName { get; set; }
        public string LocationRadius { get; set; }
        public string LocationMapId { get; set; }
        public int UserId { get; set; }

        public UserDatas User { get; set; }
        public List<LocationCars> LocationCars { get; set; }
    }
}
