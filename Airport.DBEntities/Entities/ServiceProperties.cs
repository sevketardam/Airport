using Airport.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Airport.DBEntities.Entities
{
    public class ServiceProperties : IEntity
    {
        [Key]
        public int Id { get; set; }
        public string ServicePropertyName { get; set; }
        public int ServiceCategoryId { get; set; }
        public string ServicePropertyDescription { get; set; }

        public List<ServiceItems> ServiceItems { get; set; }
        public ServiceCategories ServiceCategory { get; set; }
    }
}
