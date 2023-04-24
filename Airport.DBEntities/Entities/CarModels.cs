using Airport.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Airport.DBEntities.Entities
{
    public class CarModels : IEntity
    {
        [Key]
        public int Id { get; set; }
        public string CarModelName { get; set; }
        public int CarBrandId { get; set; }

        public CarBrands CarBrand { get; set; }
        public List<CarSeries> CarSeries { get; set; }
    }
}
