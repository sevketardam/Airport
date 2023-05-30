using Airport.DBEntities.Entities;

namespace Airport.UI.Models.VM
{
    public class LocationIsOutVM
    {
        public Locations Location { get; set; }
        public bool IsOutZone { get; set; }
        public bool IsOutZoneOutside { get; set; }
    }

    public class LocationIsOutMiniVM
    {
        public int LocationCarId { get; set; }
        public bool IsOutZone { get; set; }
        public bool IsOutZoneOutside { get; set; }
    }
}
