using Airport.DBEntities.Entities;

namespace Airport.UI.Models.VM
{
    public class ReservationStepThreeVM
    {
        public UserDatas? User { get; set; }
        public ReservationDatasVM? SelectedData { get; set; }
    }
}
