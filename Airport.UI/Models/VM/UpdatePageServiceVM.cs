using Airport.DBEntities.Entities;
using System.Collections.Generic;

namespace Airport.UI.Models.VM
{
    public class UpdatePageServiceVM
    {
        public Services Service { get; set; }
        public List<ServiceCategories> ServiceCategories { get; set; }
        public List<ServiceProperties> ServiceSelectedProperties { get; set; }
    }
}
