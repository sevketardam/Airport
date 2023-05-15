using Airport.DBEntities.Entities;
using System.Collections.Generic;

namespace Airport.UI.Models.VM
{
    public class ReservationStepThreeVM
    {
        public UserDatas? User { get; set; }
        public ReservationDatasVM? SelectedData { get; set; }
        public List<PriceServiceList> ServiceItems { get; set; }
        public Reservations UpdateReservation { get; set; }
        public List<ServiceItems> UpdateServiceItem{ get; set; }
    }

    public class PriceService
    {
        public ServiceCategories Category { get; set; }
        public ServiceProperties CategoryProp { get; set; }
    }


    public class PriceServiceList
    {
        public ServiceCategories Category { get; set; }
        public IEnumerable<ServiceProperties> CategoryProps { get; set; }
    }
}
