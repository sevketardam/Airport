using Airport.DBEntities.Context;
using Airport.DBEntities.Entities;
using Airport.DBEntitiesDAL.Interfaces;
using Airport.EF;

namespace Airport.DBEntitiesDAL.Concrete;

public class WithdrawalRequestDAL(AirportContext context) : EfRepo<AirportContext, WithdrawalRequest>(context), IWithdrawalRequestDAL
{
    public void Dispose()
    {

    }
}
