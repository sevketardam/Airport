using System;

namespace Airport.UI.Models.IM;

public class GetResevationIM
{
    public string PickValue { get; set; }
    public string PickValueText { get; set; }
    public string DropValue { get; set; }
    public string DropValueText { get; set; }
    public DateTime FlightTime { get; set; }
    public int PeopleCount { get; set; }
    public bool ReturnStatus { get; set; }
    public DateTime ReturnDate { get; set; }
}
