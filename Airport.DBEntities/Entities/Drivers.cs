using Airport.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Airport.DBEntities.Entities;

public class Drivers : IEntity
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }
    public string Phone { get; set; }
    public bool Booking { get; set; }
    public bool Financial { get; set; }
    public int UserId { get; set; }
    public string Surname { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string? PhotoFront { get; set; }
    public string? PhotoBack { get; set; }
    public string? DriverId { get; set; }
    public bool IsDelete { get; set; }

    public UserDatas User { get; set; }
    public List<MyCars> MyCars { get; set; }
    public List<Reservations> Reservations { get; set; }
    public LoginAuth LoginAuth { get; set; }
}
