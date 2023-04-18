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
        public string ServicePropertyCategory { get; set; }

        public List<ServiceItems> ServiceItems { get; set; }
    }
}
