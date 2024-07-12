using Airport.DBEntities.Context;
using Airport.DBEntities.Entities;
using Airport.DBEntitiesDAL.Interfaces;
using Airport.EF;

namespace Airport.DBEntitiesDAL.Concrete;

public class MyCarsDAL(AirportContext context) : EfRepo<AirportContext, MyCars>(context), IMyCarsDAL
{
    public void Dispose()
    {

    }
}
