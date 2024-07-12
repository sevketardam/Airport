using Airport.DBEntities.Context;
using Airport.DBEntities.Entities;
using Airport.DBEntitiesDAL.Interfaces;
using Airport.EF;

namespace Airport.DBEntitiesDAL.Concrete;

public class ServiceItemsDAL(AirportContext context) : EfRepo<AirportContext, ServiceItems>(context), IServiceItemsDAL
{
    public void Dispose()
    {

    }
}
