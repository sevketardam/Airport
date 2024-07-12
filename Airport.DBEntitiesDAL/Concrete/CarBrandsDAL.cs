using Airport.DBEntities.Context;
using Airport.DBEntities.Entities;
using Airport.DBEntitiesDAL.Interfaces;
using Airport.EF;
using System;

namespace Airport.DBEntitiesDAL.Concrete;

public class CarBrandsDAL(AirportContext context) : EfRepo<AirportContext, CarBrands>(context), ICarBrandsDAL
{
    public void Dispose()
    {

    }
}
