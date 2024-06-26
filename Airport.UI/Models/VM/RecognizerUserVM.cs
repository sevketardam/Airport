using Airport.DBEntities.Entities;

namespace Airport.UI.Models.VM;

public class RecognizerUserVM
{
    public UserDatas? User { get; set; }
    public Drivers? Driver { get; set; }
    public string Name { get; set; }
    public int Type { get; set; }
}
