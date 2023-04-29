using Airport.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Airport.DBEntities.Entities
{
    public class CarTrims : IEntity
    {
        [Key]
        public int Id { get; set; }
        public string CarTrimName { get; set; }
        public int CarSeriesId { get; set; }
        public int CarModelId { get; set; }

        public CarSeries CarSeries { get; set; }
        public CarModels CarModels { get; set; }
        public List<MyCars> MyCars { get; set; }
    }
}
