using System.Collections.Generic;

namespace Airport.UI.Models.VM;

public class GetLocationCarsPriceVM
{
    public int CarId { get; set; }
    public List<CarsPricePerKm> CarsPricePerKm { get; set; }
}
