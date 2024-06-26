using Airport.Data;
using Airport.DBEntities.Entities;
using System;

namespace Airport.DBEntitiesDAL.Interfaces;

public interface ICarTypesDAL : ISelectableRepo<CarTypes>, IEInsertableRepo<CarTypes>, IUpdatetableRepo<CarTypes>, IDeletableRepo<CarTypes>, IDisposable;
