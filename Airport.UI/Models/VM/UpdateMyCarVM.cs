using Airport.DBEntities.Entities;
using System.Collections.Generic;

namespace Airport.UI.Models.VM
{
    public class UpdateMyCarVM
    {
        public int Id { get; set; }
        public IEnumerable<CarBrands> Brands { get; set; }
        public int BrandId { get; set; }
        public IEnumerable<CarModels> Models { get; set; }
        public int ModelId { get; set; }
        public IEnumerable<CarSeries> Series { get; set; }
        public int SeriesId { get; set; }
        public IEnumerable<CarTypes> Types { get; set; }
        public int? TypeId { get; set; }
        public int MaxPassenger { get; set; }
        public int SuitCase { get; set; }
        public int SmallBags { get; set; }
        public IEnumerable<Services> Services { get; set; }
        public int? ServiceId { get; set; }
        public int? DriverId { get; set; }

        public IEnumerable<Drivers> Drivers { get; set; }

        public bool Wifi { get; set; }
        public bool Water { get; set; }
        public bool Charger { get; set; }
        public bool Disabled { get; set; }
        public bool Armored { get; set; }
        public bool Partition { get; set; }
    }
}
