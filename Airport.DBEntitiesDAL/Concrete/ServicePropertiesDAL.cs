using Airport.DBEntities.Context;
using Airport.DBEntities.Entities;
using Airport.DBEntitiesDAL.Interfaces;
using Airport.EF;

namespace Airport.DBEntitiesDAL.Concrete;

public class ServicePropertiesDAL(AirportContext context) : EfRepo<AirportContext, ServiceProperties>(context), IServicePropertiesDAL
{
    public void Dispose()
    {

    }
}
