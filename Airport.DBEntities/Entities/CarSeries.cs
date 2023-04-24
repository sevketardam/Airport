using Airport.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Airport.DBEntities.Entities
{
    public class CarSeries : IEntity
    {
        [Key]
        public int Id { get; set; }
        public string CarSeriesName { get; set; }
        public int CarModelId { get; set; }

        public CarModels CarModel { get; set; }
        public List<CarTrims> CarTrims { get; set; }
        public List<MyCars> MyCars { get; set; }
    }
}
