using System;

namespace Airport.MessageExtension.VM;

public class ReservationMailVM
{
    public string ReservationCode { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public double DropPrice { get; set; }
    public string Id { get; set; }
    public DateTime ReservationDate { get; set; }
    public string PickUpText { get; set; }
    public string DropText { get; set; }
    public string Comment { get; set; }
    public double ServiceFee { get; set; }
}
