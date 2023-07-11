using Airport.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Airport.DBEntities.Entities
{
    public class GlobalSettings : IEntity
    {
        [Key]
        public int Id { get; set; }
        public double PartnerPer { get; set; }
        public double SalesPer { get; set; }
        public double GlobalPer { get; set; }
        public DateTime LastChange { get; set; }
    }
}
