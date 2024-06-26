using Airport.Data;
using Airport.DBEntities.Entities;
using System;

namespace Airport.DBEntitiesDAL.Interfaces;

public interface ICouponsDAL : ISelectableRepo<Coupons>, IEInsertableRepo<Coupons>, IUpdatetableRepo<Coupons>, IDeletableRepo<Coupons>, IDisposable;