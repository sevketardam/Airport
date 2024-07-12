using Airport.DBEntities.Context;
using Airport.DBEntities.Entities;
using Airport.DBEntitiesDAL.Interfaces;
using Airport.EF;

namespace Airport.DBEntitiesDAL.Concrete;

public class PaymentDetailDAL(AirportContext context) : EfRepo<AirportContext, PaymentDetail>(context), IPaymentDetailDAL
{
    public void Dispose()
    {

    }
}
