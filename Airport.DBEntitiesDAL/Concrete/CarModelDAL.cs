using Airport.DBEntities.Context;
using Airport.DBEntities.Entities;
using Airport.DBEntitiesDAL.Interfaces;
using Airport.EF;

namespace Airport.DBEntitiesDAL.Concrete;

public class CarModelDAL(AirportContext context) : EfRepo<AirportContext, CarModels>(context), ICarModelsDAL
{
    public void Dispose()
    {

    }
}
