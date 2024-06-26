using Airport.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Airport.DBEntities.Entities;

public class Coupons : IEntity
{
    [Key]
    public int Id { get; set; }
    public string CouponCode { get; set; }
    public DateTime CouponStartDate { get; set; }
    public DateTime CouponFinishDate { get; set; }
    public int CouponLimit { get; set; }
    public bool Active { get; set; }
    public bool IsPerma { get; set; }
    public DateTime CreatedDate { get; set; }
    public int UserId { get; set; }
    public double Discount { get; set; }
    public string Comment { get; set; }
    public int UsingCount { get; set; }
    public bool IsOffer { get; set; }

    public UserDatas User{ get; set; }
    public List<Reservations> Reservations { get; set; }
}
