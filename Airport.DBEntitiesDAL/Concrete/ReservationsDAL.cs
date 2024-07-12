using Airport.DBEntities.Context;
using Airport.DBEntities.Entities;
using Airport.DBEntitiesDAL.Interfaces;
using Airport.EF;

namespace Airport.DBEntitiesDAL.Concrete;

public class ReservationsDAL(AirportContext context) : EfRepo<AirportContext, Reservations>(context), IReservationsDAL
{
    public void Dispose()
    {

    }
}
