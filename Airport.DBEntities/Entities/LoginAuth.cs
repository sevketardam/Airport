using Airport.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Airport.DBEntities.Entities
{
    public class LoginAuth : IEntity
    {
        [Key]
        public int Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public byte Type { get; set; }
        public int UserId { get; set; }
        public int DriverId { get; set; }

        public UserDatas User { get; set; }
        public Drivers Driver { get; set; }
    }
}
