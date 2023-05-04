using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Airport.MessageExtension.VM
{
    public class ReservationMailVM
    {
        public string ReservationCode { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Price { get; set; }
        public string Id { get; set; }
    }
}
