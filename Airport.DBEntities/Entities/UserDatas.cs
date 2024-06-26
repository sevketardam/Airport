using Airport.Data;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Airport.DBEntities.Entities;

public class UserDatas : IEntity
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }
    public string PhoneNumber { get; set; }
    public string? Profession { get; set; }
    public string? Address { get; set; }
    public string? CompanyWebsite { get; set; }
    public string? Linkedin { get; set; }
    public string? Facebook { get; set; }
    public string? TransferRequest { get; set; }
    public string? TransferRequestLocation { get; set; }
    public string? CompanyPhoneNumber { get; set; }
    public string? CompanyName { get; set; }
    public string? CompanyEmail { get; set; }
    public string? Country { get; set; }
    public string? AboutUs { get; set; }
    public string? Title { get; set; }
    public string RealPhone { get; set; }
    public string? CompanyRealPhone { get; set; }
    public string? Img { get; set; }

    public List<Services> Services { get; set; }
    public List<MyCars> MyCars { get; set; }
    public List<Drivers> Drivers { get; set; }
    public List<Locations> Locations { get; set; }
    public List<ServiceCategories> ServiceCategories{ get; set; }
    public List<Reservations> Reservations { get; set; }
    public List<Coupons> Coupons { get; set; }
    public List<WithdrawalRequest> WithdrawalRequests { get; set; }
    public UserDocs Docs { get; set; }
    public LoginAuth LoginAuth { get; set; }
}
