using Airport.Data;
using System;
using System.ComponentModel.DataAnnotations;


namespace Airport.DBEntities.Entities;

public class WithdrawalRequest : IEntity
{
    [Key]
    public int Id { get; set; }
    public int UserId { get; set; }
    public bool? Status { get; set; }
    public double Price { get; set; }
    public DateTime Date { get; set; }

    public UserDatas User { get; set; }
}
