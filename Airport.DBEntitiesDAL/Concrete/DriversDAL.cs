using Airport.DBEntities.Context;
using Airport.DBEntities.Entities;
using Airport.DBEntitiesDAL.Interfaces;
using Airport.EF;

namespace Airport.DBEntitiesDAL.Concrete;

public class DriversDAL(AirportContext context) : EfRepo<AirportContext, Drivers>(context), IDriversDAL
{
    public void Dispose()
    {

    }
}
