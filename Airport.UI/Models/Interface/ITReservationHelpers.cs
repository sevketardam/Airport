using Airport.DBEntities.Entities;
using Airport.UI.Models.IM;
using Airport.UI.Models.VM;
using System.Collections.Generic;

namespace Airport.UI.Models.Interface;

public interface ITReservationHelpers
{
    ReservationPriceVM ReservationPrice(int locationCarId, double locationKM, bool isSale, double extraServiceFee, bool returnStatus,bool isOutZone, string couponCode = null, double? specialPrice = null);

    Reservations GetReservationAll(int id);
    GetReservationValues CreateGetReservationValues(LocationCars b, double price, GetResevationIM reservation, GetGoogleAPIVM pickLocationValues, GetGoogleAPIVM dropLocationValues, List<Reservations> rate);
}
