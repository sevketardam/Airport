using Airport.DBEntities.Context;
using Airport.DBEntities.Entities;
using Airport.DBEntitiesDAL.Interfaces;
using Airport.EF;

namespace Airport.DBEntitiesDAL.Concrete;

public class LoginAuthDAL(AirportContext context) : EfRepo<AirportContext, LoginAuth>(context), ILoginAuthDAL
{
    public void Dispose()
    {
        
    }
}
