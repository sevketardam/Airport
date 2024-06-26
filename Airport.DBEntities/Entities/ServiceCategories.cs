using Airport.Data;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Airport.DBEntities.Entities;

public class ServiceCategories : IEntity
{
    [Key]
    public int Id{ get; set; }
    public string ServiceCategoryName { get; set; }
    public int UserId { get; set; }

    public List<ServiceProperties> ServiceProperties { get; set; }
    public UserDatas User { get; set; }
}
