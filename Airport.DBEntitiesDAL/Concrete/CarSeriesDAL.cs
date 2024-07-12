using Airport.DBEntities.Context;
using Airport.DBEntities.Entities;
using Airport.DBEntitiesDAL.Interfaces;
using Airport.EF;
namespace Airport.DBEntitiesDAL.Concrete;

public class CarSeriesDAL(AirportContext context) : EfRepo<AirportContext, CarSeries>(context), ICarSeriesDAL
{
    public void Dispose()
    {

    }
}
