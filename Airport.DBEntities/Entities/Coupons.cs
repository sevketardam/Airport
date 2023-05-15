using Airport.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Airport.DBEntities.Entities
{
    public class Coupons : IEntity
    {
        [Key]
        public int Id { get; set; }
        public string CouponName { get; set; }
        public DateTime CouponStartDate { get; set; }
        public DateTime CouponFinishDate { get; set; }
        public int CouponLimit { get; set; }
        public bool Active { get; set; }
        public DateTime CreatedDate { get; set; }
        public int UserId { get; set; }
        public double Discount { get; set; }

        public UserDatas User{ get; set; }
    }
}
