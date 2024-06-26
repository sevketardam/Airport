using Airport.Data;
using Airport.DBEntities.Entities;
using System;

namespace Airport.DBEntitiesDAL.Interfaces;

public interface ICarModelsDAL : ISelectableRepo<CarModels>, SelectableAsyncRepo<CarModels>, IEInsertableRepo<CarModels>, IUpdatetableRepo<CarModels>, IDeletableRepo<CarModels>, IDisposable;
