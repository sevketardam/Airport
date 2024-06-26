using Airport.Data;
using System.ComponentModel.DataAnnotations;


namespace Airport.DBEntities.Entities;

public class LoginAuth : IEntity
{
    [Key]
    public int Id { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public byte Type { get; set; }
    //0=global admin,1=müşteri,2=partner acente,3=sürücü,4=junior admin,5=satış acentesi
    
    public int UserId { get; set; }
    public int DriverId { get; set; }

    public UserDatas User { get; set; }
    public Drivers Driver { get; set; }
}
