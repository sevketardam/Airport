using Airport.DBEntities.Context;
using Airport.DBEntities.Entities;
using Airport.DBEntitiesDAL.Interfaces;
using Airport.EF;

namespace Airport.DBEntitiesDAL.Concrete;

public class ServicesDAL(AirportContext context) : EfRepo<AirportContext, Services>(context), IServicesDAL
{
    public void Dispose()
    {

    }
}
