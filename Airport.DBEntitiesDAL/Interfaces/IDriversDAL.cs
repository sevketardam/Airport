using Airport.Data;
using Airport.DBEntities.Entities;
using System;

namespace Airport.DBEntitiesDAL.Interfaces;

public interface IDriversDAL : ISelectableRepo<Drivers>, IEInsertableRepo<Drivers>, IUpdatetableRepo<Drivers>, IDeletableRepo<Drivers>, IDisposable;