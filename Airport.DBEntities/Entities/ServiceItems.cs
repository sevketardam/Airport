using Airport.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Airport.DBEntities.Entities
{
    public class ServiceItems : IEntity
    {
        [Key]
        public int Id { get; set; }
        public int ServiceId { get; set; }
        public int ServicePropertyId { get; set; }
        public string Price { get; set; }


        public Services Service { get; set; }
        public ServiceProperties ServiceProperty { get; set; }
    }
}
