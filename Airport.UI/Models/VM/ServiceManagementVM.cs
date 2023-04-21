using Airport.DBEntities.Entities;
using System.Collections.Generic;

namespace Airport.UI.Models.VM
{
    public class ServiceManagementVM
    {
        public List<ServiceProperties> ServiceProperties { get; set; }
        public List<ServiceCategories> ServiceCategories { get; set; }
    }
}
