using Airport.DBEntities.Context;
using Airport.DBEntities.Entities;
using Airport.DBEntitiesDAL.Interfaces;
using Airport.EF;

namespace Airport.DBEntitiesDAL.Concrete;

public class ServiceItemsDAL : EfRepo<AirportContext, ServiceItems>, IServiceItemsDAL
{
    public void Dispose()
    {

    }
}
