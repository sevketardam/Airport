using Airport.DBEntities.Context;
using Airport.DBEntities.Entities;
using Airport.DBEntitiesDAL.Interfaces;
using Airport.EF;

namespace Airport.DBEntitiesDAL.Concrete;

public class GlobalSettingsDAL(AirportContext context) : EfRepo<AirportContext, GlobalSettings>(context), IGlobalSettingsDAL
{
    public void Dispose()
    {

    }
}
