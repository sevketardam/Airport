using Airport.Data;
using Airport.DBEntities.Entities;
using System;

namespace Airport.DBEntitiesDAL.Interfaces;

public interface IWithdrawalRequestDAL : ISelectableRepo<WithdrawalRequest>, IEInsertableRepo<WithdrawalRequest>, IUpdatetableRepo<WithdrawalRequest>, IDeletableRepo<WithdrawalRequest>, IDisposable;