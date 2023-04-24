using Airport.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Airport.DBEntities.Entities
{
    public class CarBrands : IEntity
    {
        [Key]
        public int Id { get; set; }
        public string CarBrandName { get; set; }

        public List<CarModels> CarModels { get; set; }
    }
}
