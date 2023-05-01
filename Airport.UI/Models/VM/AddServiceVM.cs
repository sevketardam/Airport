using System.Collections.Generic;

namespace Airport.UI.Models.VM
{
    public class AddServiceVM
    {
        public string ServiceName { get; set; }
        public string ServiceDescription { get; set; }
        public List<ServicePriceDataVM> PriceData { get; set; }
    }
}
