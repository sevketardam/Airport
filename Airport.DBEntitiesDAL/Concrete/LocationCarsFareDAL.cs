using Airport.DBEntities.Context;
using Airport.DBEntities.Entities;
using Airport.DBEntitiesDAL.Interfaces;
using Airport.EF;

namespace Airport.DBEntitiesDAL.Concrete;

public class LocationCarsFareDAL(AirportContext context) : EfRepo<AirportContext, LocationCarsFare>(context), ILocationCarsFareDAL
{
    public void Dispose()
    {
        
    }
}
