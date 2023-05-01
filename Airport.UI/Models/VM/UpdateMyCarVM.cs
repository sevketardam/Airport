using Airport.DBEntities.Entities;
using System.Collections.Generic;

namespace Airport.UI.Models.VM
{
    public class UpdateMyCarVM
    {
        public int Id { get; set; }
        public List<CarBrands> Brands { get; set; }
        public int BrandId { get; set; }
        public List<CarModels> Models { get; set; }
        public int ModelId { get; set; }
        public List<CarSeries> Series { get; set; }
        public int SeriesId { get; set; }
        public List<CarTrims> Trims { get; set; }
        public int TrimId { get; set; }
        public List<CarClasses> Classes { get; set; }
        public int? ClassId { get; set; }
        public List<CarTypes> Types { get; set; }
        public int? TypeId { get; set; }
        public int MaxPassenger { get; set; }
        public int SuitCase { get; set; }
        public int SmallBags { get; set; }
        public List<Services> Services { get; set; }
        public int? ServiceId { get; set; }

        public bool Wifi { get; set; }
        public bool Water { get; set; }
        public bool Charger { get; set; }
        public bool Disabled { get; set; }
        public bool Armored { get; set; }
        public bool Partition { get; set; }
    }
}
