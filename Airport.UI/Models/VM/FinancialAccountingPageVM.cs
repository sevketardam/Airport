using Airport.DBEntities.Entities;
using System.Collections.Generic;

namespace Airport.UI.Models.VM
{
    public class FinancialAccountingPageVM
    {
        public List<Reservations> Reservation { get; set; }
        public bool IsPendingRequest { get; set; }
        public decimal RequestPrice { get; set; } = 0;
    }
}
