using Airport.Data;
using Airport.DBEntities.Entities;
using System;

namespace Airport.DBEntitiesDAL.Interfaces;

public interface ILoginAuthDAL : ISelectableRepo<LoginAuth>, IEInsertableRepo<LoginAuth>, IUpdatetableRepo<LoginAuth>, IDeletableRepo<LoginAuth>, IDisposable;
