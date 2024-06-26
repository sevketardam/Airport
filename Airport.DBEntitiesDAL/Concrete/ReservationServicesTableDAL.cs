using Airport.DBEntities.Context;
using Airport.DBEntities.Entities;
using Airport.DBEntitiesDAL.Interfaces;
using Airport.EF;

namespace Airport.DBEntitiesDAL.Concrete;

public class ReservationServicesTableDAL : EfRepo<AirportContext, ReservationServicesTable>, IReservationServicesTableDAL
{
    public void Dispose()
    {
        
    }
}
