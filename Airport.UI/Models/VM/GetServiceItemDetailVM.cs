using System.Collections.Generic;

namespace Airport.UI.Models.VM
{
    public class GetServiceItemDetailVM
    {
        public int ServiceCategoryId { get; set; }
        public string ServiceCategoryName { get; set; }
        public List<GetServiceCategoryItemVM> CategoryItems { get; set; } 
    }
}
