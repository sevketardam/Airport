using Airport.Data;
using System;
using System.ComponentModel.DataAnnotations;

namespace Airport.DBEntities.Entities;

public class PaymentDetail : IEntity
{
    [Key]
    public int Id { get; set; }
    public string PaymentStatusCode { get; set; }
    public string PaymentStatusText { get; set; }
    public string ReservationCode { get; set; }
    public int ReservationId { get; set; }
    public string PaymentPrice { get; set; }
    public DateTime Date { get; set; }
    public string POSTDate { get; set; }


    public Reservations Reservation { get; set; }
}
