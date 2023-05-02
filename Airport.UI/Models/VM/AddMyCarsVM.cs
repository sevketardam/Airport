using Airport.DBEntities.Entities;
using System.Collections.Generic;

namespace Airport.UI.Models.VM
{
    public class AddMyCarsVM
    {
        public List<CarBrands> CarBrands{ get; set; }
        public List<Drivers> Drivers { get; set; }
        public List<Services> ServiceItems { get; set; }
    }
}
