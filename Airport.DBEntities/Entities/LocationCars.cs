using Airport.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Airport.DBEntities.Entities
{
    public class LocationCars : IEntity
    {
        [Key]
        public int Id { get; set; }
        public int LocationId { get; set; }
        public string Price { get; set; }
        public int CarId { get; set; }

        public Locations Location { get; set; }
        public MyCars Car { get; set; }
    }
}
