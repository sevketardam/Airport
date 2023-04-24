using Airport.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Airport.DBEntities.Entities
{
    public class CarTypes : IEntity
    {
        [Key]
        public int Id { get; set; }
        public string CarTypeName { get; set; }
        public int CarTrimId { get; set; }

        public CarTrims CarTrim { get; set; }
        public List<MyCars> MyCars { get; set; }
    }
}
