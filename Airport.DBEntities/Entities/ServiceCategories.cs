using Airport.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Airport.DBEntities.Entities
{
    public class ServiceCategories : IEntity
    {
        [Key]
        public int Id{ get; set; }
        public string ServiceCategoryName { get; set; }

        public List<ServiceProperties> ServiceProperties { get; set; }
    }
}
