using Airport.DBEntities.Context;
using Airport.DBEntities.Entities;
using Airport.DBEntitiesDAL.Interfaces;
using Airport.EF;

namespace Airport.DBEntitiesDAL.Concrete;

public class CouponsDAL(AirportContext context) : EfRepo<AirportContext, Coupons>(context), ICouponsDAL
{
    public void Dispose()
    {

    }
}
