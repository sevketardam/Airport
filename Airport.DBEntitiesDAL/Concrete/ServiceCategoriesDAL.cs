using Airport.DBEntities.Context;
using Airport.DBEntities.Entities;
using Airport.DBEntitiesDAL.Interfaces;
using Airport.EF;

namespace Airport.DBEntitiesDAL.Concrete;

public class ServiceCategoriesDAL : EfRepo<AirportContext, ServiceCategories>, IServiceCategoriesDAL
{
    public void Dispose()
    {

    }
}
