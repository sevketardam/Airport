using Airport.DBEntities.Context;
using Airport.DBEntities.Entities;
using Airport.DBEntitiesDAL.Interfaces;
using Airport.EF;

namespace Airport.DBEntitiesDAL.Concrete;

public class PaymentDetailDAL : EfRepo<AirportContext, PaymentDetail>, IPaymentDetailDAL
{
    public void Dispose()
    {

    }
}
