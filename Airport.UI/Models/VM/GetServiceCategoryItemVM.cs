namespace Airport.UI.Models.VM;

public class GetServiceCategoryItemVM
{
    public int Id { get; set; }
    public string ServiceName { get; set; }
    public string ServiceCategoryName { get; set; }
    public string ServiceDescripton { get; set; }
    public int ServiceCategoryId { get; set; }
    public double? Price { get; set; } 

}
