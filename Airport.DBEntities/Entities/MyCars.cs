using Airport.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Airport.DBEntities.Entities
{
    public class MyCars : IEntity
    {
        [Key]
        public int Id { get; set; }
        public int BrandId { get; set; }
        public int ModelId { get; set; }
        public int SeriesId { get; set; }
        public int? TypeId { get; set; }
        public int MaxPassenger { get; set; }
        public int SuitCase { get; set; }
        public int SmallBags { get; set; }
        public int? ServiceId { get; set; }
        public int UserId { get; set; }

        public bool Wifi { get; set; }
        public bool Water { get; set; }
        public bool Charger { get; set; }
        public bool Disabled { get; set; }
        public bool Armored { get; set; }
        public bool Partition { get; set; }
        public int? DriverId { get; set; }

        public CarBrands Brand { get; set; }
        public Drivers Driver { get; set; }
        public CarModels Model { get; set; }
        public CarSeries Series { get; set; }
        public CarTypes Type { get; set; }
        public Services Service { get; set; }
        public UserDatas User { get; set; }
        public List<LocationCars> LocationCars { get; set; }

    }
}
