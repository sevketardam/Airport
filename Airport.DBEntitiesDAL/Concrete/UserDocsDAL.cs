using Airport.DBEntities.Context;
using Airport.DBEntities.Entities;
using Airport.DBEntitiesDAL.Interfaces;
using Airport.EF;

namespace Airport.DBEntitiesDAL.Concrete;

public class UserDocsDAL(AirportContext context) : EfRepo<AirportContext, UserDocs>(context), IUserDocsDAL
{
    public void Dispose()
    {

    }
}
