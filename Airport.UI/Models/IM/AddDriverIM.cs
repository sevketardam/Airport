using System;

namespace Airport.UI.Models.IM
{
    public class AddDriverIM
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public bool Booking { get; set; }
        public bool Financial { get; set; }
        public string Surname { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Photo { get; set; }
        public string DriverId { get; set; }
    }
}
