using Airport.DBEntities.Context;
using Airport.DBEntities.Entities;
using Airport.DBEntitiesDAL.Interfaces;
using Airport.EF;

namespace Airport.DBEntitiesDAL.Concrete;

public class ReservationPeopleDAL(AirportContext context) : EfRepo<AirportContext, ReservationPeople>(context), IReservationPeopleDAL
{
    public void Dispose()
    {

    }
}
