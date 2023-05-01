using Airport.DBEntities.Entities;
using System.Collections.Generic;

namespace Airport.UI.Models.VM
{
    public class ServiceCategoryPriceVM
    {
        public double Price { get; set; }
        public int PropId { get; set; }
        public int CategoryId { get; set; }
        public ServiceProperties ServiceProperties { get; set; }
    }


}
