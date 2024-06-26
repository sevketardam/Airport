using Airport.DBEntities.Entities;
using System.Collections.Generic;
using System.Linq;

namespace Airport.UI.Models.VM;

public class UpdatePageServiceVM
{
    public Services Service { get; set; }
    public List<ServiceCategories> ServiceCategories { get; set; }
    public List<ServiceProperties> ServiceSelectedProperties { get; set; }
    public List<ServiceLastCategoryPriceVM> ServicePriceDatas { get; set; }
}

public class ServiceLastCategoryPriceVM
{
    public int CategoryId { get; set; }
    public string CategoryName{ get; set; }
    public IEnumerable<ServiceCategoryPriceVM> serviceCategoryPrices { get; set; }
}
