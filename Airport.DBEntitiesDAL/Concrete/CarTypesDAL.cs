using Airport.DBEntities.Context;
using Airport.DBEntities.Entities;
using Airport.DBEntitiesDAL.Interfaces;
using Airport.EF;

namespace Airport.DBEntitiesDAL.Concrete;

public class CarTypesDAL(AirportContext context) : EfRepo<AirportContext, CarTypes>(context), ICarTypesDAL
{
    public void Dispose()
    {

    }
}
