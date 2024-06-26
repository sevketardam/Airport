using System.Collections.Generic;

namespace Airport.UI.Models.VM;

public class ServicePriceDataVM
{
    public int CategoryId { get; set; }
    public List<ServiceCategoryPriceVM> PriceData { get; set; }
}
