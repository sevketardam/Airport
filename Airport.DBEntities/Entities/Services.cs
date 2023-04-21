using Airport.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Airport.DBEntities.Entities
{
    public class Services : IEntity
    {
        [Key]
        public int Id { get; set; }
        public string ServiceName { get; set; }
        public string ServiceDescription { get; set; }
        public int UserId { get; set; }


        public UserDatas User { get; set; }
        public List<ServiceItems> ServiceItems { get; set; }
    }
}
