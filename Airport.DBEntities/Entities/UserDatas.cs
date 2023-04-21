using Airport.Data;
using Airport.DBEntities;
using Airport.DBEntities.Context;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Airport.DBEntities.Entities
{
    public class UserDatas : IEntity
    {
        [Key]
        public int Id { get; set; }
        public string Eposta { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string? Lastname { get; set; }
        public string PhoneNumber { get; set; }
        public byte Type { get; set; }


        public List<Services> Services { get; set; }
    }
}
