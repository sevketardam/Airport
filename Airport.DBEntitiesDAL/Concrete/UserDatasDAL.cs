using Airport.DBEntities.Context;
using Airport.DBEntities.Entities;
using Airport.DBEntitiesDAL.Interfaces;
using Airport.EF;

namespace Airport.DBEntitiesDAL.Concrete;

public class UserDatasDAL(AirportContext context) : EfRepo<AirportContext, UserDatas>(context), IUserDatasDAL
{
    public void Dispose()
    {

    }

}
