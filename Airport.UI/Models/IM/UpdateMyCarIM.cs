using Airport.DBEntities.Entities;
using System.Collections.Generic;

namespace Airport.UI.Models.IM
{
    public class UpdateMyCarIM
    {
        public int Brand { get; set; }
        public int Model { get; set; }
        public int Series { get; set; }
        public int? Type { get; set; }
        public int MaxPassenger { get; set; }
        public int SuitCase { get; set; }
        public int SmallBags { get; set; }
        public int? Service { get; set; }
        public int? Driver { get; set; }

        public bool Wifi { get; set; }
        public bool Water { get; set; }
        public bool Charger { get; set; }
        public bool Disabled { get; set; }
        public bool Armored { get; set; }
        public bool Partition { get; set; }
    }
}
