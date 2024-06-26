using Airport.Data;
using System;
using System.ComponentModel.DataAnnotations;

namespace Airport.DBEntities.Entities;

public class UserDocs : IEntity
{
    [Key]
    public int Id { get; set; }
    public int UserId { get; set; }
    public string? Docs1 { get; set; }
    public string? Docs2 { get; set; }
    public string? Docs3 { get; set; }
    public bool Docs1Status { get; set; }
    public bool Docs2Status { get; set; }
    public bool Docs3Status { get; set; }
    public DateTime LastUpdate { get; set; }
    public bool Docs1AdminStatus { get; set; }
    public bool Docs2AdminStatus { get; set; }
    public bool Docs3AdminStatus { get; set; }


    public UserDatas User { get; set; }
}
