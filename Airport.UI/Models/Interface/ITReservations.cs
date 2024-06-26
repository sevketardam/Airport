using Airport.UI.Models.IM;
using Airport.UI.Models.VM;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Airport.UI.Models.Interface;

public interface ITReservations
{
    Task<List<LocationIsOutVM>> GetLocationIsOutList(List<AllDatas> allData);
    Task<List<AllDatas>> GetLocationAllDataList(GetGoogleAPIVM pickLocationValues, GetGoogleAPIVM dropLocationValues);
    Task<GetReservationListVM> ReservationList(List<LocationIsOutVM> locationIsOutVM, GetResevationIM reservation, double minKm, GetGoogleAPIVM pickLocationValues, GetGoogleAPIVM dropLocationValues);
}
