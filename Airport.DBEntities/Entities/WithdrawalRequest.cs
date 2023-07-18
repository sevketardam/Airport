using Airport.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Airport.DBEntities.Entities
{
    public class WithdrawalRequest : IEntity
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        public bool? Status { get; set; }
        public double Price { get; set; }
        public DateTime Date { get; set; }

        public UserDatas User { get; set; }
    }
}
